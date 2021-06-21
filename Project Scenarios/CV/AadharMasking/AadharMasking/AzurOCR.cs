using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace AzureOCR
{
    
    

    public class AzurImageOCR
        {
        private string subscriptionKey = ConfigurationManager.AppSettings["OCRSubscriptionKey"], Endpoint = ConfigurationManager.AppSettings["OCREndpoint"];
        public List<IList<int>> OCRBoxList = new List<IList<int>>();
        public float Angle;
        public int Width;
        public int Height;

        private const TextRecognitionMode textRecognitionMode = TextRecognitionMode.Printed;
        private const int numberOfCharsInOperationId = 36;
        //Variable to append the OCR Results from SDK
        public string Error = "";/*, OcrResult = "", StoreType = "";*/

        public async Task OcrImage(string data)
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
            Angle = (result.RecognitionResults.Count > 0) ? (float)result.RecognitionResults[0].ClockwiseOrientation : 0;
            Width = (result.RecognitionResults.Count > 0) ? (int)result.RecognitionResults[0].Width : 0;
            Height= (result.RecognitionResults.Count > 0) ? (int)result.RecognitionResults[0].Height : 0;
            foreach (TextRecognitionResult recResult in recResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    if(line.Words.Count==3 && line.Text.Length==14 && line.Text.Replace(" ", string.Empty).All(char.IsDigit)){
                        OCRBoxList.Add(line.BoundingBox);
                    }
                }
            }
        }

        ////Retriving the recognized text
        //private bool GetText(byte[] imgBytes)
        //{
        //    var client = new RestClient(Endpoint+"/vision/v2.0/ocr");
        //    var request = new RestRequest(Method.POST);

        //    request.AddHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
        //    request.AddHeader("Content-Type", "application/octet-stream");
        //    request.AddParameter("undefined", imgBytes, ParameterType.RequestBody);
        //    IRestResponse response = client.Execute(request);

        //    dynamic OcrData = JObject.Parse(response.Content);

        //    foreach (JProperty prop in OcrData.Properties())
        //        if (prop.Name == "message")
        //        {
        //            Error = OcrData.message;
        //            return false;
        //        }
        //        else if (prop.Name == "error")
        //        {
        //            dynamic ImgError = JObject.Parse(OcrData.error.ToString());
        //            Error = ImgError.message;
        //            return false;
        //        }

        //    JArray TextList = JArray.Parse(OcrData.regions.ToString());

        //    for (int i = 0; i < TextList.Count; i++)
        //    {
        //        dynamic LineList = JObject.Parse(TextList[i].ToString());
        //        JArray Lines = JArray.Parse(LineList.lines.ToString());

        //        for (int j = 0; j < Lines.Count; j++)
        //        {
        //            dynamic Line = JObject.Parse(Lines[j].ToString());
        //            JArray Words = JArray.Parse(Line.words.ToString());

        //            string LineText = "";
        //            for (int k = 0; k < Words.Count; k++)
        //            {
        //                dynamic Word = JObject.Parse(Words[k].ToString());
        //                LineText +=Word.text.ToString() + " ";
        //            }
        //            OCRList.Add(LineText);
        //            OcrResult += LineText + " <br> ";
        //        }
        //    }
        //    return true;
        //}
        
    }
}
       