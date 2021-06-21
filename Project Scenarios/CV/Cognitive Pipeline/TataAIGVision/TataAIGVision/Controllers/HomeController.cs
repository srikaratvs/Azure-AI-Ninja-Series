using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TataAIGVision.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
           
             
                return View();
        }

        public ActionResult Process()
        {
            return View();
        }

            //Login Check Function
            public JsonResult LoginCheck(string user_name, string password)
        {
            try
            {
                if (user_name == "Admin" && password == "JSN123!")
                {
                   
                    return Json(new { Result = "100" });
                }
                else
                {
                    return Json(new { Result = "200" });
                }

            }
            catch (Exception e)
            {
                return Json(new { Result = e.Message });
            }
        }
        //Logout Check Function
        public JsonResult LogoutCheck()
        {
            try
            {
                Session.Clear();
                Session.Abandon();

                return Json(new { Result = "100" });
            }
            catch (Exception e)
            {
                return Json(new { Result = e.Message });
            }
        }

        //POST: Recognize Text 
        [HttpPost]
        public async Task<JsonResult> TataAIGVision(string data)
        {
            string str, typeofImage;
            int char_initial_index = 500;

            Luis ls = new Luis();
            List<string> TextList = new List<string>();

            try
            {
                RecognizeText rt = new RecognizeText();
                await rt.ExtractText(data);

                if (rt.Error == "")
                {
                    str = rt.rtResult;
                    str = str.Replace("LLP*#*#*#*#", "LLP********");

                    if ((str.Contains("DEMAND")) || (str.Contains("ON DEMAND PAY")) || (str.Contains("DEMAND DRAF")) || (str.Contains("MANAGER'S CHEQUE")))
                    {
                        typeofImage = "Demand Draft";
                    }
                    else if ((str.Contains("Permanent Account Number")) || (str.Contains("Permiment Account Number")))
                    {
                        //typeofImage = "PAN";
                        return Json(new { StatusCode = "S", Type = "PAN" });
                    }
                    else
                    {
                        typeofImage = "Cheque";
                    }

                    if (str.Length >= 500)
                    {

                        for (int TIn = 0; TIn < str.Length;)
                        {
                            string OutText;
                            if (TIn + char_initial_index >= str.Length)
                            {
                                OutText = str.Substring(TIn, str.Length - TIn);
                                TIn = str.Length;
                                TextList.Add(OutText);
                            }
                            else
                            {
                                OutText = str.Substring(TIn, char_initial_index);

                                int len = OutText.LastIndexOf("/ ");
                                //if (len == -1)
                                //    len = OutText.LastIndexOf(", ");
                                if (len == -1)
                                    len = char_initial_index - 2;
                                len += 2;

                                if (len <= OutText.Length)
                                    OutText = OutText.Substring(0, len);
                                else
                                    OutText = OutText.Substring(0, len - 2);
                                TIn += len;
                                TextList.Add(OutText);
                            }
                        }
                        for (int i = 0; i < TextList.Count; i++)
                        {
                            ls.DoLuis(TextList[i].ToString());
                        }
                        //string[] luisinput = TextList.ToArray();
                    }
                    else
                    {
                        ls.DoLuis(str);
                    }
                    return Json(new { StatusCode = "S", Type = typeofImage, CustomerName = ls.CustomerName, BankName = ls.BankName, Date = ls.Date, AmountInWords = ls.AmountInWords, AmountInDigits = ls.AmountInDigits, FullJSON = rt.result });
                }
                return Json(new { StatusCode = "F", Message = rt.Error });
            }
            catch (Exception e)// handling runtime errors and returning error as Json
            {
                return Json(new { StatusCode = "F", Message = e.Message });
            }
        }
    }
}