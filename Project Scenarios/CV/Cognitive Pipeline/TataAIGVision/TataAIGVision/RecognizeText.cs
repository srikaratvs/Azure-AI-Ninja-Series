using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using System.Net;

namespace TataAIGVision
{
    public class Luis
    {
        public string CustomerName = null, BankName = null, Date = null, AmountInWords = null, AmountInDigits=null;

        public void DoLuis(string Text)
        {

            var client = new RestClient(ConfigurationManager.AppSettings["LUIS_EndPoint"] + ConfigurationManager.AppSettings["LUIS_AppID"] + "?verbose=true&timezoneOffset=-360&subscription-key=" + ConfigurationManager.AppSettings["LUIS_Key"] + "&q=" + Text);

            //var client = new RestClient("https://tataaigluis.cognitiveservices.azure.com/luis/prediction/v3.0/apps/dd161cbe-04a1-4b3c-868e-5ec56f968288/slots/production/predict?subscription-key=557824771e464b5798546308751ac8ee&verbose=true&show-all-intents=true&log=true&query=" + Text);

            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic JsonResult = JsonConvert.DeserializeObject(response.Content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                for (int k = 0; k < JsonResult.entities.Count; k++)
                {
                    if (JsonResult.entities[k]["type"].ToString() == "PayeeName")
                        CustomerName = JsonResult.entities[k]["entity"].ToString();
                    else if (JsonResult.entities[k]["type"].ToString() == "BankName")
                        BankName = JsonResult.entities[k]["entity"].ToString();
                    else if (JsonResult.entities[k]["type"].ToString() == "Date")
                        Date = JsonResult.entities[k]["entity"].ToString();
                    else if (JsonResult.entities[k]["type"].ToString() == "AmountInWords")
                        AmountInWords = JsonResult.entities[k]["entity"].ToString();
                    else if (JsonResult.entities[k]["type"].ToString() == "AmountInDigits")
                        AmountInDigits = JsonResult.entities[k]["entity"].ToString();
                }
            }
            return;
        }
    }

    public class RecognizeText
    {

        private string subscriptionKey = ConfigurationManager.AppSettings["ExtractedText_SubscriptionKey"], Endpoint = ConfigurationManager.AppSettings["ExtractedText_Endpoint"];

        //For printed text
        private const TextRecognitionMode textRecognitionMode = TextRecognitionMode.Printed;
        private const int numberOfCharsInOperationId = 36;
        //Variable to append the OCR Results from SDK
        public string Error = "";
        public List<string> RTList = new List<string>();
        public string rtResult = "";

        public ReadOperationResult result;

        public async Task ExtractText(string data)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey), new System.Net.Http.DelegatingHandler[] { });
            //Endpoint
            computerVision.Endpoint = Endpoint;

            //Image data to Byte Array
            byte[] imageBytes = Convert.FromBase64String(data);

            //Byte Array To Stream
            Stream stream = new MemoryStream(imageBytes);

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
            result = await computerVision.GetReadOperationResultAsync(operationId);

            //Waiting for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running || result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                await Task.Delay(200);

                result = await computerVision.GetReadOperationResultAsync(operationId);
            }

            //Displaying the results
            var recResults = result.RecognitionResults;

            bool flag = true;
            foreach (TextRecognitionResult recResult in recResults)
            {
                foreach (Line line in recResult.Lines)
                {
                    if (flag)
                        flag = false;
                    else
                    {
                        RTList.Add(line.Text);
                        rtResult += " " + line.Text;
                    }
                }
            }
        }
    }
}