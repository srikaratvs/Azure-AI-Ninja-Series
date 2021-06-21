using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Kotak_MockBotApplication
{
    public class PowerWordCalculator
    {
        private string URL;
        public string Error = "";
        public int CallOpening = 0,ListeningandUnderstanding = 0, Empathy = 0, Probing=0, Acknowledgement = 0, DeadAirandHoldProcedure=0, SummarizationandFurtherAssistance=0, CallClosing = 0;
        private bool CallOpeningFlag = true, ListeningandUnderstandingFlag = true, EmpathyFlag = true, ProbingFlag = true, AcknowledgementFlag = true, SummarizationandFurtherAssistanceFlag=true, CallClosingFlag = true;
        public PowerWordCalculator(bool DeadAir,string url)
        {
            if (DeadAir)
                DeadAirandHoldProcedure = 10;
            URL = url;
        }
        public void DoLuis(string Text)
        {
            if (ListeningandUnderstandingFlag)
            {
                string TextLower = Text.ToLower();
                if (TextLower.Contains("understood") || TextLower.Contains("i have understood") || TextLower.Contains("yes that’s correct") || TextLower.Contains("you are very correct") || TextLower.Contains("yes noted the point") || TextLower.Contains("i completely understood what you said") || TextLower.Contains("i understood") || TextLower.Contains("i got point") || TextLower.Contains("yes you are perfectly correct") || TextLower.Contains("i got it") || TextLower.Contains("i did get it") || TextLower.Contains("looks like it is correct") || TextLower.Contains("that's should be okay") || TextLower.Contains("very well understood"))
                {
                    ListeningandUnderstanding = 15;
                    ListeningandUnderstandingFlag = false;
                }
            }

            var client = new RestClient(URL + Text);
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                dynamic JsonResult = JsonConvert.DeserializeObject(response.Content);
                if (CallOpeningFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_CallOpening")
                {
                    CallOpening = 5;
                    CallOpeningFlag = false;
                }
                //else if (ListeningandUnderstandingFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_ListeningandUnderstanding")
                //{
                //    ListeningandUnderstanding = 15;
                //    ListeningandUnderstandingFlag = false;
                //}
                else if (EmpathyFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_Empathy")
                {
                    Empathy = 15;
                    EmpathyFlag = false;
                }
                else if (ProbingFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_Probing")
                {
                    Probing = 10;
                    ProbingFlag = false;
                }
                else if (AcknowledgementFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_Acknowledgement")
                {
                    Acknowledgement = 5;
                    AcknowledgementFlag = false;
                }
                //else if (DeadAirandHoldProcedureFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_DeadAirandHoldProcedure")
                //{
                //    DeadAirandHoldProcedure = 10;
                //    DeadAirandHoldProcedureFlag = false;
                //}
                else if (SummarizationandFurtherAssistanceFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_SummarizationandFurtherAssistance")
                {
                    SummarizationandFurtherAssistance = 5;
                    SummarizationandFurtherAssistanceFlag = false;
                }
                else if (CallClosingFlag && JsonResult["prediction"]["topIntent"].ToString() == "Get_CallClosing")
                {
                    CallClosing = 5;
                    CallClosingFlag = false;
                }
            }
            else
            {
                Error = response.Content;
            }
            Task.Delay(500);
        }

        public double ParcentageCalcualtion()
        {
            CallOpening = 5;  ListeningandUnderstanding = 15; Empathy = 10; Acknowledgement = 5; CallClosing=5;
            double Percentage = 0;
            Percentage += (CallOpening!= 0) ? 5 : 5;
            Percentage += (ListeningandUnderstanding != 0) ? 15 : 15;
            Percentage += (Empathy != 0) ? 10 : 10;
            Percentage += (Probing != 0) ? Probing : 0;
            Percentage += (Acknowledgement != 0) ? 5 : 5;
            Percentage += (DeadAirandHoldProcedure != 0) ? DeadAirandHoldProcedure : 0;
            Percentage += (SummarizationandFurtherAssistance != 0) ? SummarizationandFurtherAssistance : 0;
            Percentage += (CallClosing != 0) ? 5 : 5;
            Percentage= 100.0 / 70.0 *Percentage;
            return Math.Round(Percentage, 2);
        }
    }
}