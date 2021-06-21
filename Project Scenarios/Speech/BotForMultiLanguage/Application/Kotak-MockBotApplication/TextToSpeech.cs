using RestSharp;
using System;
using System.Xml.Linq;

namespace LiveSpeechDemo
{
    public class TextToSpeech
    {
        private string TTSEndPoint, TokenEndPoint, SpeechKey;
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public TextToSpeech() { }

        public TextToSpeech(string TTSEndpoint, string TokeEndpoint, string SubscriptionKey)
        {
            this.TTSEndPoint = TTSEndpoint;
            this.SpeechKey = SubscriptionKey;
            this.TokenEndPoint = TokeEndpoint;
        }

        public TextToSpeech GetSpeechFromText(string Text, string language)
        {
            string VoiceName = "", VoiceStyle ="";

            if(language == "ta")//Tamil
            {
                VoiceName = "ta-IN"; // Male
                VoiceStyle = "ta-IN-Valluvar";
            }
            else if (language == "hi")// Hindi
            {
                VoiceName = "hi-IN";
                VoiceStyle = "hi-IN-Hemant";
            }
            else if (language == "te")// Telugu
            {
                VoiceName = "te-IN";
                VoiceStyle = "te-IN-Chitra";
            }
            else //English
            {
                VoiceName = "en-US";
                VoiceStyle = "en-US-BenjaminRUS";
            }

            IRestResponse Tokenresponse = GetToken();
            if (Tokenresponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var client = new RestClient(TTSEndPoint);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Authorization", "Bearer " + Tokenresponse.Content);
                request.AddHeader("Content-Type", "application/ssml+xml");
                request.AddHeader("X-Microsoft-OutputFormat", "riff-24khz-16bit-mono-pcm");
                request.AddHeader("User-Agent", "YOUR_RESOURCE_NAME");
                request.AddParameter("undefined", GetConfig(Text, language, VoiceName, VoiceStyle), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return new TextToSpeech { StatusCode = 200, Message = "data:audio/wav;base64, " + Convert.ToBase64String(response.RawBytes) };
                else
                    return new TextToSpeech { StatusCode = 400, Message = response.StatusCode.ToString() };
            }
            else
                return new TextToSpeech { StatusCode = 400, Message = Tokenresponse.StatusCode.ToString() };
        }

        private IRestResponse GetToken()
        {
            //Calling Python Gates Service
            var client = new RestClient(TokenEndPoint);
            var request = new RestRequest(Method.POST);
            request.AddHeader("Ocp-Apim-Subscription-Key", SpeechKey);
            return client.Execute(request);           
        }

        private string GetConfig(string text,string lang, string VoiceName, string VoiceStyle)
        {
            var ssmlDoc = new XDocument(
                                  new XElement("speak",
                                      new XAttribute("version", "1.0"),
                                      new XAttribute(XNamespace.Xml + "lang", VoiceName),
                                      new XElement("voice",
                                          new XAttribute(XNamespace.Xml + "lang", VoiceName),
                                          new XAttribute("name", VoiceStyle), text))); //Short name for 'Microsoft Server Speech Text to Speech Voice (en-IN-Ravi-Apollo)
            return ssmlDoc.ToString();
        }
    }
}