using LiveSpeechDemo;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using VoiceGestureRecognition;


namespace Kotak_MockBotApplication.Controllers
{
    public class HomeController : Controller
    {
        public int StatusCode { get; set; }
        public string Text { get; set; }
        public string Voice { get; set; }
       
        private static string STTEndpoint = ConfigurationManager.AppSettings["STTEndpoint"], SpeechKey = ConfigurationManager.AppSettings["SpeechKey"]; // Getting Speech Endpoint and subscription  key
        private static string TTSEndpoint = ConfigurationManager.AppSettings["TTSEndpoint"], TokenEndpoint = ConfigurationManager.AppSettings["TokenEndpoint"];
      
        private static string TTEndpoint = ConfigurationManager.AppSettings["TTEndpoint"], TTKey = ConfigurationManager.AppSettings["TTKey"];

        public ActionResult login()
        {
            Session.Clear();
            Session.Abandon();
            return View();
        }
       
        public ActionResult Chat(string Mode="English", string language= "en-IN")
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("login");
            }
            else
            {
                ViewBag.Mode = Mode;
                ViewBag.language = language;
                return View();
            }          
        }

        public JsonResult LoginCheck(string Login, string Password)
        {
            try
            {
                if (Login == "Admin" && Password == "JSN123!")
                {
                    Session["UserID"] = Login;
                    return Json(new { StatusCode = "200", Message = "Login Successful" });
                }
                return Json(new { StatusCode = "500", Message = "Invalid Username and Password" });
            }
            catch (Exception e)// handling runtime errors and returning error as Json
            {
                return Json(new { StatusCode = "500", Message = e.Message });
            }
        }



        [HttpPost]
        public ActionResult GetText(string Text, string Language,int Count)
        {
            if (Language == "en-IN")
            {
                if (Count == 1)
                {
                    string Message = "Hi, Can you help me with Chennai Quarantine Facilities ?";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if(Count == 2)
                {
                    string Message = "I am having mild fever and tested positive 2 days back ";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if (Count == 3)
                {
                    string Message = "It is 277487179013";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if (Count == 4)
                {
                    string Message = "Egmore , Chennai";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if (Count == 5)
                {
                    string Message = "7823875668";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if (Count == 6)
                {
                    string Message = "Okay";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if (Count == 7)
                {
                    string Message = "Can you send the details to my mobile number?";
                    return Json(GetVoice(Message, 200, 0, Language));
                }
                else if (Count == 8)
                {
                    string Message = "Thank you.";
                    return Json(GetVoice(Message, 200, 0, Language));
                }                
                else
                {
                    return Json(new { StatusCode = 400 });
                }
            }            
            else
            {
                return Json(new { StatusCode = 400 });
            }
        }



        
        private string EnglishToOther(string text, string language)
        {
            TextTranslation TTrans = new TextTranslation(TTEndpoint, TTKey);
            TTrans.GetTextTranslationAsync(text, language).Wait();
            return TTrans.Result;
        }

        private string OthersToEnglish(string text)
        {
            TextTranslation TTrans = new TextTranslation(TTEndpoint, TTKey);
            TTrans.GetTextTranslationAsync(text, "en-IN").Wait();
            return TTrans.Result;
        }
    
        private Object GetVoice(string Text, int code = 200,int Tim=0,string lag="en-IN", string TranslatedText=null)
        {
            TextToSpeech tts = new TextToSpeech(TTSEndpoint, TokenEndpoint, SpeechKey).GetSpeechFromText(Text,lag);
            if (code == 201)
                return new { StatusCode = code, Text = Text, Voice = tts.Message, Time = Tim };
            else
                return (tts.StatusCode == 200) ? new { StatusCode = code, Text = Text, Voice = tts.Message, TranslatedText= TranslatedText } : new { StatusCode = 400, Text = tts.Message, Voice = tts.Message, TranslatedText = TranslatedText };
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                //  Get all files from Request object  
                HttpFileCollectionBase files = Request.Files;
                //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                //string filename = Path.GetFileName(Request.Files[0].FileName);  
                //string fname = Path.Combine(Server.MapPath("~/Uploads/"), "Test.wav");
                //files[0].SaveAs(fname);
                Byte[] Content = getByte(files[0].InputStream);
                //Byte[] Content = new BinaryReader(files[0].InputStream).ReadBytes(files[0].ContentLength);
                SpeechToText stt = new SpeechToText(STTEndpoint, SpeechKey);
                return Json(stt.GetTextFromSpeech(Content));
            }
            else
                return Json(new { StatusCode = 400, Message = "No Voice Found" });
        }

        private byte[] getByte(Stream sourceStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                sourceStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}