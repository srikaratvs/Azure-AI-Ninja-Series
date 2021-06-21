using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MetroConstruction.Controllers
{
    public class empdata
    {
        public string user_id { get; set; }
        public string employee_id { get; set; }
        public string register_image1 { get; set; }
        public string first_name { get; set; }
        public string log_type { get; set; }
        public string log_date_time { get; set; }           
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //DataGallery.GetLogData();
            ViewBag.data = get_employee_data();
            return View();
        }

        public ActionResult Aimodels()
        {
            return View();
        }
        

        

        public List<empdata> get_employee_data()
        {
            try
            {
                var client = new RestClient("");                
                var request = new RestRequest(Method.POST);
                IRestResponse response = client.Execute(request);
                List<empdata> emd = new List<empdata>();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dynamic json = JsonConvert.DeserializeObject(response.Content.ToString());  
                    if(json["StatusCode"]== "API200")
                    {
                        for(int i = 0; i < json["employee_data"].Count; i++)
                        {
                            emd.Add(
                                new empdata
                                {
                                    user_id =json["employee_data"][i]["user_id"],
                                    employee_id= json["employee_data"][i]["employee_id"],
                                    register_image1= "data:image/jpeg;base64, "+ Convert.ToString(json["employee_data"][i]["register_image1"]),
                                    first_name= json["employee_data"][i]["first_name"],
                                    log_type= json["employee_data"][i]["log_type"],
                                    log_date_time= json["employee_data"][i]["log_date_time"]
                                }
                                );
                        }
                    }
                    return emd;                    
                }
                else
                {
                    return new List<empdata>();
                }
            }
            catch (Exception e) //handling runtime errors
            {
                return new List<empdata>();
            }
        }



        //public object StartCamera(string Event)
        public object GetEmpData()
        {
            try
            {                    
                var client = new RestClient("/get_employee_data.php");
                var request = new RestRequest(Method.POST);
                
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //dynamic json = JsonConvert.DeserializeObject(response.Content.ToString());                    
                    return Content(response.Content.ToString(), "application/json");
                    //return Json(JsonConvert.SerializeObject(json));
                }
                else
                {
                    return Json(new { StatusCode = "API400", Message = ""+(int)response.StatusCode+"-"+response.Content });
                }                
            }
            catch (Exception e) //handling runtime errors
            {
                return Json(new { StatusCode = "API400", Message = e.Message });
            }
        }




        public object NonFoodFMCG(string image)
        {
            try
            {
                byte[] ms = Convert.FromBase64String(image);
                bool Planogram;
                List<String> AllReorders = new List<String>();
                List<decimal> All_Scores = new List<decimal>();
                List<string> All_Labels = new List<string>();
                List<decimal> Reds = new List<decimal>();
                List<decimal> Blues = new List<decimal>();
                List<decimal> Greens = new List<decimal>();
                List<string> Red_Labels = new List<string>();
                List<string> Blue_Labels = new List<string>();
                List<string> Green_Labels = new List<string>();
                var client = new RestClient("https://dynamictesting.cognitiveservices.azure.com/customvision/v3.0/Prediction//detect/iterations/Iteration2/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", " application/octet-stream");
                request.AddParameter(" application/octet-stream", ms, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);
                ArrayList predictions = data["predictions"];
                foreach (Dictionary<string, dynamic> item in predictions)
                {
                    decimal Threshold = 0.30M;
                    if (item["probability"] >= Threshold)
                    {
                        var prediction_scores = item["boundingBox"];
                        All_Scores.Add(prediction_scores["top"]);
                        All_Labels.Add(item["tagName"]);
                    }
                }

                //Console.WriteLine(All_Scores);
                //Console.WriteLine(All_Labels);
                foreach (var it in All_Labels.Select((x, i) => new { Value = x, Index = i }))
                {
                    if (it.Value == "Red")
                    {
                        Red_Labels.Add(it.Value);
                        Reds.Add(All_Scores[it.Index]);
                    }
                    else if (it.Value == "Blue")
                    {
                        Blue_Labels.Add(it.Value);
                        Blues.Add(All_Scores[it.Index]);
                    }
                    else if (it.Value == "Green")
                    {
                        Green_Labels.Add(it.Value);
                        Greens.Add(All_Scores[it.Index]);
                    }
                }
                
                if (Reds.Count <= 10)
                {
                    AllReorders.Add("Red");
                }
                if (Greens.Count <= 10)
                {
                    AllReorders.Add("Green");
                }
                if (Blues.Count <= 10)
                {
                    AllReorders.Add("Blue");
                }
                if ((Reds.Count <= 0) || (Greens.Count <= 0) || (Blues.Count <= 0))
                {
                    return Json(new { ResponseCode = 200, Planogram = false, ReOrders = AllReorders });
                }
                double RedComplains = (double)Reds.Max() - (double)Reds.Min();
                double GreenComplains = (double)Greens.Max() - (double)Greens.Min();
                double BlueComplains = (double)Blues.Max() - (double)Blues.Min();
                
                if ((RedComplains < 0.13) && (GreenComplains < 0.13) && (BlueComplains < 0.13))
                {
                    Planogram = true;
                }
                else
                {
                    Planogram = false;
                }
                return Json(new { ResponseCode = 200, Planogram = Planogram, ReOrders = AllReorders });
            }
            catch (Exception e) //handling runtime errors
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }
        }
        
        public object FoodFMCG(string image)
        //public async System.Threading.Tasks.Task<Dictionary<bool, bool>> Post()
        {
            try
            {

                //int Rack_1_Y_Range = 250;
                //int Rack_2_Y_Range1 = 260;
                //int Rack_2_Y_Range2 = 490;
                //int Rack_3_Y_Range = 500;

                List<string> Rack1_Objects = new List<string>();     //For Bottles
                List<string> Rack2_Objects = new List<string>();     //For MilkCans
                List<string> Rack3_Objects = new List<string>();     //For Chips

                List<string> PlanogramCheck = new List<string>();
                List<string> ReOrderCheck = new List<string>();
                List<string> AllReOrders = new List<string>();


                // below 5 lists For new approach 
                List<decimal> All_Scores = new List<decimal>();
                List<string> All_Labels = new List<string>();
                List<decimal> Bottles = new List<decimal>();
                List<decimal> MilkCans = new List<decimal>();
                List<decimal> Chips = new List<decimal>();


                Dictionary<string, dynamic> resp_dict = new Dictionary<string, dynamic>();

                byte[] ImageData = Convert.FromBase64String(image);


                var client = new RestClient("https://fmcg-poc.cognitiveservices.azure.com/customvision/v3.0/Prediction//detect/iterations/Iteration4/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", " application/octet-stream");
                request.AddParameter(" application/octet-stream", ImageData, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);

                ArrayList predictions = data["predictions"];

                foreach (Dictionary<string, dynamic> item in predictions)
                {
                    decimal Threshold = 0.21M;
                    if (item["probability"] >= Threshold)
                    {

                        // This Logic is for Saying Objcts in which Rack:

                        //var prediction_scores = item["boundingBox"];
                        //All_Scores.Add(prediction_scores["top"]);
                        //int y1 = Convert.ToInt32(prediction_scores["top"] * 720);
                        //if (y1 < Rack_1_Y_Range)
                        //{
                        //    Rack1_Objects.Add(item["tagName"]);
                        //}
                        //if (y1 > Rack_2_Y_Range1 && y1 < Rack_2_Y_Range2)
                        //{
                        //    Rack2_Objects.Add(item["tagName"]);
                        //}
                        //if (y1 > Rack_3_Y_Range)
                        //{
                        //    Rack3_Objects.Add(item["tagName"]);
                        //}
                        var prediction_scores = item["boundingBox"];
                        All_Scores.Add(prediction_scores["top"]);
                        All_Labels.Add(item["tagName"]);
                        /*                        if (prediction_scores["top"] >= 0.5M && item["tagName"] == "Bottle")
                                                {
                                                    Console.WriteLine(true);
                                                }*/
                    }
                }



                foreach (var it in All_Labels.Select((x, i) => new { Value = x, Index = i }))
                {
                    if (it.Value == "Bottle")
                    {
                        Rack1_Objects.Add(it.Value);
                        Bottles.Add(All_Scores[it.Index]);
                    }
                    else if (it.Value == "MilkCan")
                    {
                        Rack2_Objects.Add(it.Value);
                        MilkCans.Add(All_Scores[it.Index]);
                    }
                    else if (it.Value == "Chips")
                    {
                        Rack3_Objects.Add(it.Value);
                        Chips.Add(All_Scores[it.Index]);
                    }
                }

                if ((Rack1_Objects.Count <= 0) || (Rack2_Objects.Count <= 0) || (Rack3_Objects.Count <= 0))
                {
                    return Json(new { ResponseCode = 200, Planogram = false, ReOrders = AllReOrders });
                }


                // For PlanogramComplaince  If need to tell particular Rack
                //if (((Rack2_Objects.Contains("Bottle") && Rack3_Objects.Contains("Bottle")) || Rack2_Objects.Contains("Bottle") || Rack3_Objects.Contains("Bottle")) || ((Rack1_Objects.Contains("MilkCan") && Rack3_Objects.Contains("MilkCan")) || Rack1_Objects.Contains("MilkCan") || Rack3_Objects.Contains("MilkCan")) || ((Rack1_Objects.Contains("Chips") && Rack2_Objects.Contains("Chips")) || Rack1_Objects.Contains("Chips") || Rack2_Objects.Contains("Chips")))
                //{
                //    PlanogramCheck.Add("PlanogramComplaince");
                //}
                //else
                //{
                //    PlanogramCheck.Add("NoPlanogramComplaince");
                //}


                // For Re-Order
                int HeadCount = 10;
                if ((Rack1_Objects.Count >= HeadCount) && (Rack2_Objects.Count >= HeadCount) && (Rack3_Objects.Count >= HeadCount))
                {
                    ReOrderCheck.Add("NoReOrder");
                }
                else
                {
                    //ReOrderCheck.Add("ReOrder");
                    if (Rack1_Objects.Count < HeadCount)
                    {
                        ReOrderCheck.Add("Bottle");
                        AllReOrders.Add("Bottles");
                    }
                    if (Rack2_Objects.Count < HeadCount)
                    {
                        ReOrderCheck.Add("MilkCan");
                        AllReOrders.Add("MilkCans");
                    }
                    if (Rack3_Objects.Count < HeadCount)
                    {
                        ReOrderCheck.Add("Chips");
                        AllReOrders.Add("Chips");
                    }

                }

                // Planogram Complaince for new Approach without checking in which Rack
                double BottleMax = (double)Bottles.Max();
                double BottleMin = (double)Bottles.Min();
                double MilkCanMax = (double)MilkCans.Max();
                double MilkCanMin = (double)MilkCans.Min();
                double ChipsMax = (double)Chips.Max();
                double ChipsMin = (double)Chips.Min();
                double BottleComplains = BottleMax - BottleMin;
                double MilkCanComplains = MilkCanMax - MilkCanMin;
                double ChipsComplains = ChipsMax - ChipsMin;
                if ((BottleComplains > 0.1 && BottleComplains < 0.8) || (MilkCanComplains > 0.1 && MilkCanComplains < 0.8) || (ChipsComplains > 0.1 && ChipsComplains < 0.8))
                {
                    PlanogramCheck.Add("PlanogramComplaince");
                }
                else
                {
                    PlanogramCheck.Add("NoPlanogramComplaince");
                }

                //if (PlanogramCheck.Contains("PlanogramComplaince") && ReOrderCheck.Contains("ReOrder"))
                //{
                //    resp_dict.Add("Status", "Suceeded");
                //    resp_dict.Add("PlanogramComplaince", true);
                //    resp_dict.Add("ReOrder", true);
                //}
                //else if (PlanogramCheck.Contains("PlanogramComplaince") && ReOrderCheck.Contains("NoReOrder"))
                //{
                //    resp_dict.Add("Status", "Suceeded");
                //    resp_dict.Add("PlanogramComplaince", true);
                //    resp_dict.Add("ReOrder", false);
                //}
                //else if (PlanogramCheck.Contains("NoPlanogramComplaince") && ReOrderCheck.Contains("ReOrder"))
                //{
                //    resp_dict.Add("Status", "Suceeded");
                //    resp_dict.Add("PlanogramComplaince", false);
                //    resp_dict.Add("ReOrder", true);
                //}
                //else
                //{
                //    resp_dict.Add("Status", "Suceeded");
                //    resp_dict.Add("PlanogramComplaince", false);
                //    resp_dict.Add("ReOrder", false);
                //}

                if (PlanogramCheck.Contains("PlanogramComplaince") && ReOrderCheck.Contains("NoReOrder"))
                {                    
                    return Json(new { ResponseCode = 200, Planogram = false, ReOrders = AllReOrders });
                }
                else if (PlanogramCheck.Contains("NoPlanogramComplaince") && ReOrderCheck.Contains("NoReOrder"))
                {
                    return Json(new { ResponseCode = 200, Planogram = true, ReOrders = AllReOrders });
                }

                else if ((PlanogramCheck.Contains("PlanogramComplaince")) && (ReOrderCheck.Contains("Bottle") || ReOrderCheck.Contains("MilkCan") || ReOrderCheck.Contains("Chips")))
                {
                    return Json(new { ResponseCode = 200, Planogram = false, ReOrders = AllReOrders });
                }
                else
                {
                    return Json(new { ResponseCode = 200, Planogram = true, ReOrders = AllReOrders });
                }
            }
            catch(Exception e)
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }
        }

       
        public object ConstructionSafety(string image)
        //public async System.Threading.Tasks.Task<Dictionary<bool, bool>> Post()
        {
            try
            {
                List<string> All_Labels = new List<string>();
                List<string> Not_Missing_Equipments = new List<string>();
                List<string> Missing_Equipments = new List<string>();



                Dictionary<string, dynamic> resp_dict = new Dictionary<string, dynamic>();

                byte[] ImageData = Convert.FromBase64String(image); 

                var client = new RestClient("https://fmcg-classify.cognitiveservices.azure.com/customvision/v3.0/Prediction//detect/iterations/Iteration4/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", " application/octet-stream");
                request.AddParameter(" application/octet-stream", ImageData, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);

                ArrayList predictions = data["predictions"];

                foreach (Dictionary<string, dynamic> item in predictions)
                {
                    decimal Threshold = 0.23M;
                    if (item["probability"] >= Threshold)
                    {
                        All_Labels.Add(item["tagName"]);
                    }
                }

                foreach (var it in All_Labels.Select((x, i) => new { Value = x, Index = i }))
                {
                    if (it.Value == "NoHelmet")
                    {
                        Missing_Equipments.Add("Helmet");
                    }
                    else if (it.Value == "NoVest")
                    {
                        Missing_Equipments.Add("Vest");
                    }
                    else
                    {
                        Not_Missing_Equipments.Add(it.Value);
                    }
                }

                // Make new unique list  (To remove duplicates from the original List)
                List<string> Missing_Equipments_Unique_List = Missing_Equipments.Distinct().ToList();

                if (Missing_Equipments.Contains("Helmet") || Missing_Equipments.Contains("Vest"))
                {
                    return Json(new { ResponseCode = 200, Missing = Missing_Equipments_Unique_List });
                }
                else
                {                   
                    return Json(new { ResponseCode = 200, Missing = Missing_Equipments_Unique_List });
                }
            }
            catch (Exception e)
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }
        }


        
        public object AnalyseMarks(string image)
        {
            try
            {
                byte[] ms = Convert.FromBase64String(image);
                HashSet<string> Detected_Marks = new HashSet<string>();
                var client = new RestClient("https://genericpoc.cognitiveservices.azure.com/customvision/v3.0/Prediction//detect/iterations/Iteration4/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", "application/octet-stream");
                request.AddParameter("application/octet-stream", ms, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);
                ArrayList predictions = data["predictions"];
                foreach (Dictionary<string, dynamic> item in predictions)
                {
                    decimal Threshold = 0.15M;
                    if (item["probability"] >= Threshold)
                    {
                        var tag_name = item["tagName"];
                        Detected_Marks.Add(tag_name);
                    }
                }

                List<String> Final_detected_Marks = Detected_Marks.ToList<string>();

                return Json(new { ResponseCode = 200, DetectedMarks = Final_detected_Marks });
            }
            catch (Exception e)
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }

        }

        public object AnalyseCount(string image)
        {
            try
            {
                byte[] ms = Convert.FromBase64String(image);
                int Detected_Count = 0;
                var client = new RestClient("https://pidilite-resource.cognitiveservices.azure.com/customvision/v3.0/Prediction//detect/iterations/Iteration2/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", " application/octet-stream");
                request.AddParameter(" application/octet-stream", ms, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);
                ArrayList predictions = data["predictions"];
                foreach (Dictionary<string, dynamic> item in predictions)
                {
                    decimal Threshold = 0.50M;
                    if (item["probability"] >= Threshold)
                    {
                        Detected_Count += 1;
                    }
                }

                return Json(new { ResponseCode = 200, DetectedCirclesCount = Detected_Count });
            }
            catch (Exception e)
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }

        }

        
        public object ColourIdentification(string image)
        //public async System.Threading.Tasks.Task<Dictionary<bool, bool>> Post()
        {
            try
            {
                List<string> Color = new List<string>();
                List<string> NegativeGarbage = new List<string>();

                Dictionary<string, dynamic> resp_dict = new Dictionary<string, dynamic>();

                byte[] ImageData = Convert.FromBase64String(image); 

                var client = new RestClient("https://genericpoc.cognitiveservices.azure.com/customvision/v3.0/Prediction//classify/iterations/Iteration2/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", " application/octet-stream");
                request.AddParameter(" application/octet-stream", ImageData, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);
                System.Collections.ArrayList predictions = data["predictions"];
                Dictionary<string, dynamic> Labels_Obj = data["predictions"][0];
                List<string> label = new List<string>();
                label.Add(Labels_Obj["tagName"]);
                /*                foreach (System.Collections.Generic.Dctionary<string, dynamic> item in predictions)
                                {

                                    decimal probability = Convert.ToDecimal(item["probability"]);
                                    decimal Threshold = 0.45M;
                                    if (probability >= Threshold)
                                    {
                                        Color.Add(item["tagName"]);
                                    }
                                }*/

                if (label[0] == "Negative")
                {                    
                    return Json(new { ResponseCode = 200, IdentifiedColour = NegativeGarbage });
                }
                else
                {
                    return Json(new { ResponseCode = 200, IdentifiedColour = label });
                }
            }
            catch (Exception e)
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }
        }


        // For Shape Identification:        
        public object ShapeIdentification(string image)
        //public async System.Threading.Tasks.Task<Dictionary<bool, bool>> Post()
        {
            try
            {
                List<string> Shape = new List<string>();
                List<string> NegativeGarbage = new List<string>();

                Dictionary<string, dynamic> resp_dict = new Dictionary<string, dynamic>();

                byte[] ImageData = Convert.FromBase64String(image);

                var client = new RestClient("https://fmcg-classify.cognitiveservices.azure.com/customvision/v3.0/Prediction//classify/iterations/Iteration1/image");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Prediction-Key", "");
                request.AddHeader("Content-Type", " application/octet-stream");
                request.AddParameter(" application/octet-stream", ImageData, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Dictionary<string, dynamic>>(response.Content);
                if (data == null)
                {
                    return Json(new { ResponseCode = 500, Message = "data null" });
                }
                else
                {
                    ArrayList predictions = data["predictions"];

                    Dictionary<string, dynamic> Labels_Obj = data["predictions"][0];
                    List<string> label = new List<string>();
                    label.Add(Labels_Obj["tagName"]);

                    if (label[0] == "Negative")
                    {                        
                        return Json(new { ResponseCode = 200, ShapeResult = false });
                    }
                    else
                    {
                        return Json(new { ResponseCode = 200, ShapeResult = (label[0].ToLower()=="pass")?true:false });
                    }
                }               
            }
            catch (Exception e)
            {
                return Json(new { ResponseCode = 500, Message = e.Message });
            }
        }

    }
}