using Newtonsoft.Json;
using RestSharp;

namespace VoiceGestureRecognition
{
    public class SpeechToText
    {
        private string SpeechEndPoint, SpeechKey;
        public int StatusCode{get;set;}
        public string Message { get; set; }

        public SpeechToText() { }

        public SpeechToText(string SpeechEndpoint,string SubscriptionKey) {
            this.SpeechEndPoint = SpeechEndpoint;
            this.SpeechKey = SubscriptionKey;
        }

        public SpeechToText GetTextFromSpeech(byte[] voice)
        {

            var client = new RestClient(SpeechEndPoint + "/speech/recognition/conversation/cognitiveservices/v1?language=en-IN");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Ocp-Apim-Subscription-Key", SpeechKey);
            request.AddHeader("Content-type", "audio/wav; codecs=audio/pcm; samplerate=16000");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("audio/wav; codecs=audio/pcm; samplerate=16000", voice, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);            
            if ((int)response.StatusCode == 200)
            {
                dynamic result = JsonConvert.DeserializeObject(response.Content);
                if(result.RecognitionStatus == "Success")
                    return new SpeechToText { StatusCode = 200, Message = result.DisplayText };
                else if (result.RecognitionStatus == "NoMatch")
                    return new SpeechToText { StatusCode = 400, Message = result.RecognitionStatus+ " (Speech was detected in the audio stream, but no words from the target language were matched.Usually means the recognition language is a different language from the one the user is speaking)" };
                else if (result.RecognitionStatus == "InitialSilenceTimeout")
                    return new SpeechToText { StatusCode = 400, Message = result.RecognitionStatus+ " (The start of the audio stream contained only silence, and the service timed out waiting for speech)" };
                else if (result.RecognitionStatus == "BabbleTimeout")
                    return new SpeechToText { StatusCode = 400, Message = result.RecognitionStatus + " (The start of the audio stream contained only noise, and the service timed out waiting for speech)" };
                else
                    return new SpeechToText { StatusCode = 400, Message = "Error (The recognition service encountered an internal error and could not continue. Try again if possible)" };
            }
            else if ((int)response.StatusCode == 100 )
                return new SpeechToText { StatusCode = 400, Message = "Proceed with sending the rest of the data (Used with chunked transfer)" };
            else if ((int)response.StatusCode == 400)
                return new SpeechToText { StatusCode = 400, Message = "Bad request (Language code not provided, not a supported language, invalid audio file, etc)" };
            else if ((int)response.StatusCode == 401)
                return new SpeechToText { StatusCode = 400, Message = "Unauthorized	(Subscription key is invalid in the specified region, or invalid endpoint)" };
            else if ((int)response.StatusCode == 403)
                return new SpeechToText { StatusCode = 400, Message = "Forbidden (Missing subscription key or authorization token)" };
            else
                return new SpeechToText { StatusCode = 400, Message = "Unable to access the URL" };
        }
        
    }
}