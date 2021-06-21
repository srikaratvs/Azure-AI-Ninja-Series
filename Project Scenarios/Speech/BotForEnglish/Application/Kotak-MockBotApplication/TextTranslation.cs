using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kotak_MockBotApplication
{
    public class TextTranslation
    {
        private string TTEndPoint, TTKey;
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Result { get; set; }

        public TextTranslation() { }

        public TextTranslation(string TTEndpoint, string SubscriptionKey)
        {
            this.TTEndPoint = TTEndpoint;
            this.TTKey = SubscriptionKey;
        }

        public async Task<string> GetTextTranslationAsync(string text, string language)
        {
            string route = "/translate?api-version=3.0&to=" + language;
            //string textToTranslate = "Hello, world!";
            object[] body = new object[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(TTEndPoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", TTKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "southeastasia");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();

                dynamic resobj = JsonConvert.DeserializeObject(result);
                Result = resobj[0]["translations"][0]["text"].ToString();

               
                //string rr = resobj[0]["detectedLanguage"].ToString();
                return "";
            }
        }
    }
}