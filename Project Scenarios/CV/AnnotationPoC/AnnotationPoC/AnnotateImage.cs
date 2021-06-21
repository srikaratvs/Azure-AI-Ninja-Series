using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Configuration;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;

namespace AnnotationPoC
{
    
    public class IndoorImageAnnotation
    {
        public string Error = "", Result="", Probability = "";
        //Assigning Subscription Key and Face Endpoint from web.config file
        private string subscriptionKey = ConfigurationManager.AppSettings["PredictionKey"], Endpoint = ConfigurationManager.AppSettings["EndPoint"], ProjectId = ConfigurationManager.AppSettings["ProjectId"], Iteration = ConfigurationManager.AppSettings["Iteration"];


        public void DeepImageAnnotation(string data)
        {
            try
            {
                if (data == "")
                    Error = "Image is Empty";
                else
                {
                    byte[] imageBytes = Convert.FromBase64String(data);

                    var client = new RestClient(Endpoint + "/customvision/v3.0/Prediction/" + ProjectId + "/classify/iterations/" + Iteration + "/image");
                    var request = new RestRequest(Method.POST);

                    request.AddHeader("Prediction-Key", subscriptionKey);
                    request.AddHeader("Content-Type", "application/octet-stream");
                    request.AddParameter("undefined", imageBytes, ParameterType.RequestBody);

                    IRestResponse response = client.Execute(request);
                    //int StatusCode = (int)response.StatusCode;
                    //HttpStatusCode statusCode = response.StatusCode;
                    //int numericStatusCode = (int)statusCode;

                    dynamic ImgData = JObject.Parse(response.Content);

                    foreach (JProperty prop in ImgData.Properties())
                        if (prop.Name == "message")
                        {
                            Error = ImgData.message;
                            break;
                        }
                        else if (prop.Name == "error")
                        {
                            dynamic ImgError = JObject.Parse(ImgData.error.ToString());
                            Error = ImgError.message;
                            break;
                        }
                    if (Error == "")
                    {
                        JArray Annotation = JArray.Parse(ImgData.predictions.ToString());
                        dynamic Image = JObject.Parse(Annotation[0].ToString());
                        Probability = Image.probability;
                        Result = Image.tagName;
                    }
                }
            }
            catch (Exception e)// handling runtime errors and returning error as Json
            {
                Error = e.Message;
            }
        }
    }


    public class OutdoorImageAnnotation
    {
        private string subscriptionKey = ConfigurationManager.AppSettings["OCRSubscriptionKey"], Endpoint = ConfigurationManager.AppSettings["OCREndpoint"];
        private string LuisKey=ConfigurationManager.AppSettings["LUIS_Key"],LuisEndPoint = ConfigurationManager.AppSettings["LUIS_EndPoint"], LuisAppId = ConfigurationManager.AppSettings["LUIS_AppID"];
        private List<string> OCRList = new List<string>();

        private const TextRecognitionMode textRecognitionMode = TextRecognitionMode.Printed;
        private const int numberOfCharsInOperationId = 36;
        //Variable to append the OCR Results from SDK
        public string Error = "", OcrResult = "", StoreType = "";

        public async Task OcrImageAnnotation(string data)
        {
            try
            {
                if (data == "")
                    Error = "Image is Empty";
                else
                {
                    //Image data to Byte Array
                    byte[] imageBytes = Convert.FromBase64String(data);
                    await ExtractText(imageBytes);
                    if(Error=="")
                        Luis();
                }

            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }



        private async Task ExtractText(byte[] imgBytes)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            //Endpoint
            computerVision.Endpoint = Endpoint;
                
            //Byte Array To Stream
            Stream stream = new MemoryStream(imgBytes);

            try
            {
                //Starting the async process to recognize the text
                BatchReadFileInStreamHeaders textHeaders = await computerVision.BatchReadFileInStreamAsync(stream, textRecognitionMode);

                await GetTextAsync(computerVision, textHeaders.OperationLocation);

            }
            catch (Exception e)
            {
                Error = e.Message;
            }
            
        }

        //Retriving the recognized text
        private async Task GetTextAsync(ComputerVisionClient computerVision, string operationLocation)
        {
            //Retrieve the URI where the recognized text will be stored from the Operation-Location header
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            //Calling GetReadOperationResultAsync
            ReadOperationResult result = await computerVision.GetReadOperationResultAsync(operationId);

            //Waiting for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                await Task.Delay(1000);

                result = await computerVision.GetReadOperationResultAsync(operationId);
            }

            //Displaying the results
            var recResults = result.RecognitionResults;


            foreach (TextRecognitionResult recResult in recResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    OCRList.Add(line.Text);
                    OcrResult += line.Text + " <br> ";
                }
            }
        }

        //Retriving the recognized text
        private bool GetText(byte[] imgBytes)
        {
            var client = new RestClient(Endpoint+"/vision/v2.0/ocr");
            var request = new RestRequest(Method.POST);

            request.AddHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.AddHeader("Content-Type", "application/octet-stream");
            request.AddParameter("undefined", imgBytes, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            dynamic OcrData = JObject.Parse(response.Content);

            foreach (JProperty prop in OcrData.Properties())
                if (prop.Name == "message")
                {
                    Error = OcrData.message;
                    return false;
                }
                else if (prop.Name == "error")
                {
                    dynamic ImgError = JObject.Parse(OcrData.error.ToString());
                    Error = ImgError.message;
                    return false;
                }

            JArray TextList = JArray.Parse(OcrData.regions.ToString());

            for (int i = 0; i < TextList.Count; i++)
            {
                dynamic LineList = JObject.Parse(TextList[i].ToString());
                JArray Lines = JArray.Parse(LineList.lines.ToString());

                for (int j = 0; j < Lines.Count; j++)
                {
                    dynamic Line = JObject.Parse(Lines[j].ToString());
                    JArray Words = JArray.Parse(Line.words.ToString());

                    string LineText = "";
                    for (int k = 0; k < Words.Count; k++)
                    {
                        dynamic Word = JObject.Parse(Words[k].ToString());
                        LineText +=Word.text.ToString() + " ";
                    }
                    OCRList.Add(LineText);
                    OcrResult += LineText + " <br> ";
                }
            }
            return true;
        }

        private void Luis()
        {
            //Loop
            for (int j = 0; j < OCRList.Count; j++)
            {
                string text = OCRList[j];
                //Calling LUIS
                var client = new RestClient(LuisEndPoint + LuisAppId + "?verbose=true&timezoneOffset=-360&subscription-key=" + LuisKey + "&q=" + OCRList[j]);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);

                dynamic jObject = JObject.Parse(response.Content);
                foreach (JProperty prop in jObject.Properties())
                    if (prop.Name == "message")
                    {
                        Error = jObject.message;
                        j = OCRList.Count;
                        break;
                    }
                    else if (prop.Name == "error")
                    {
                        dynamic ImgError = JObject.Parse(jObject.error.ToString());
                        Error = ImgError.message;
                        j = OCRList.Count;
                        break;
                    }
                if (Error == "")
                {
                    JArray luislenobj = JArray.Parse(jObject.entities.ToString());

                    for (int k = 0; k < luislenobj.Count; k++)
                    {
                        StoreType = jObject["entities"][k]["type"].ToString();
                        j = OCRList.Count;
                        break;
                        //if (jObject["topScoringIntent"]["intent"].ToString() == "Identify Store Type")
                        //{
                        //    StoreType = jObject["entities"][k]["type"].ToString();
                        //    j = OCRList.Count;
                        //    break;
                        //}
                    }
                }
                Thread.Sleep(210);
            }
        }
    }
}
       