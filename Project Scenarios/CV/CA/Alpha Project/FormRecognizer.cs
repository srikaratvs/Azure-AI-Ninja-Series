using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using System.Threading;
using RestSharp;
using System.Configuration;
using static Alpha_Project.BlobStorage;

namespace Alpha_Project
{
    public class FormRecognizer
    {
        private static string Classification = ConfigurationManager.AppSettings["Classification"];
        private static string Subclassification = ConfigurationManager.AppSettings["Subclassification"];
        private string FRKey = ConfigurationManager.AppSettings["FormRecognizerKey"], FREndpoint = ConfigurationManager.AppSettings["FormRecognizerEndpoint"], AnalyzeEndpoint = ConfigurationManager.AppSettings["AnalyzeEndpoint"], AnalyzeResultEndpoint = ConfigurationManager.AppSettings["AnalyzeResultEndpoint"];

        public string Error = "", jsonString = "";
        public dynamic JSONresult;
        public bool continuousDateFormatcheck =false;

        public List<string> FRList = new List<string>();
        string FRResult = null;

        bool StartFlag, ContinueWithDateCount ;
        int DateCount = 0;
       
        public List<List<string>> FRFinalList = new List<List<string>>();

        string ttype = "",classification = "";

        public void DoLuis()
        {

            try
            {
                if (FRFinalList.Count > 0)
                {
                    for (int i = 0; i < FRFinalList.Count; i++)
                    {
                        string str = FRFinalList[i][1].ToString();

                        if (str != null)
                        {
                            var client = new RestClient(Classification + "&verbose=true&show-all-intents=true&log=true&query=" + str);
                            var request = new RestRequest(Method.GET);
                            Thread.Sleep(400);
                            IRestResponse response = client.Execute(request);

                            var sub_client = new RestClient(Subclassification + "&verbose=true&show-all-intents=true&log=true&query=" + str);
                            var sub_request = new RestRequest(Method.GET);
                            Thread.Sleep(400);
                            IRestResponse sub_response = sub_client.Execute(sub_request);

                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                dynamic JsonResult = JsonConvert.DeserializeObject(response.Content);

                                if (JsonResult["prediction"]["topIntent"].ToString() == "get_Expense")
                                {
                                    ttype = "Withdrawals";
                                    classification = "Expense";
                                    FRFinalList[i][3] = ttype;
                                    FRFinalList[i][4] = classification;
                                }
                                else if (JsonResult["prediction"]["topIntent"].ToString() == "get_Income")
                                {
                                    ttype = "Deposits";
                                    classification = "Income";
                                    FRFinalList[i][3] = ttype;
                                    FRFinalList[i][4] = classification;
                                }
                                else if (JsonResult["prediction"]["topIntent"].ToString() == "get_Liability")
                                {
                                    ttype = "Deposits";
                                    classification = "Liability";
                                    FRFinalList[i][3] = ttype;
                                    FRFinalList[i][4] = classification;
                                }
                                else if (JsonResult["prediction"]["topIntent"].ToString() == "get_Asset")
                                {
                                    ttype = "Withdrawals";
                                    classification = "Asset";
                                    FRFinalList[i][3] = ttype;
                                    FRFinalList[i][4] = classification;
                                }
                                else
                                {
                                    if (JsonResult["prediction"]["topIntent"].ToString() == "Previous balance")
                                    {
                                        ttype = "Opening Balance";
                                        classification = "Suspense";
                                        FRFinalList[i][3] = ttype;
                                        FRFinalList[i][4] = classification;
                                    }
                                    else
                                    {
                                        ttype = "Withdrawals";
                                        classification = "Suspense";
                                        FRFinalList[i][3] = ttype;
                                        FRFinalList[i][4] = classification;
                                    }
                                }
                            }

                            if (sub_response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                dynamic sub_JsonResult = JsonConvert.DeserializeObject(sub_response.Content);

                                if (StringEmptyCheck(sub_JsonResult["prediction"]["topIntent"].ToString()))
                                {
                                    FRFinalList[i][5] = sub_JsonResult["prediction"]["topIntent"].ToString();
                                }
                                else
                                {
                                    FRFinalList[i][5] = "Unknown";
                                }
                            }
                        }
                       
                    }
                }            
                else
                {
                    Error = "Err, Narration list was empty";
                }
            }
            catch (Exception e)
            {
                Error = Error = "Stack Trace: " + e.StackTrace + " Exception Messgae: " + e.Message;
            }
        }

        public void GenerateText(byte[] imageBytes)
        {
            try
            {
                //byte[] imageBytes = Convert.FromBase64String(str);
                var client = new RestClient(AnalyzeEndpoint);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/octet-stream");
                request.AddHeader("Ocp-Apim-Subscription-Key", FRKey);
                request.AddParameter("application/octet-stream", imageBytes, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                if ((int)response.StatusCode == 202)
                {
                    if (response.Headers.Count > 0)
                    {
                        string operationLocation = response.Headers.ToList().Find(x => x.Name == "Operation-Location").Value.ToString();
                        var client1 = new RestClient(operationLocation);
                        client1.Timeout = -1;
                        var request1 = new RestRequest(Method.GET);
                        request1.AddHeader("Ocp-Apim-Subscription-Key", FRKey);
                        IRestResponse response1;

                        do
                        {
                            response1 = client1.Execute(request1);
                            Thread.Sleep(200);
                            JSONresult = JsonConvert.DeserializeObject(response1.Content);
                        } while ((int)response1.StatusCode == 200 && JSONresult.status != "succeeded");

                        if ((int)response1.StatusCode == 200 && JSONresult.status == "succeeded")
                        {
                            string temp = "", initialstr = "" , strCurrent="", amount="";

                            //JSONresult = JsonConvert.DeserializeObject(JSONresult);

                            jsonString = JsonConvert.SerializeObject(JSONresult);
                            //var pageResult = JSONresult.analyzeResult.pageResults;

                            var readResult = JSONresult.analyzeResult.readResults;

                            for (int i = 0; i < readResult.Count; i++)
                            {
                                StartFlag = false;
                                ContinueWithDateCount = true;
                                DateCount = 0;

                                for (int k = 0; k < readResult[i].lines.Count; k++)
                                {
                                    FRList.Add((string)readResult[i].lines[k].text);
                                    //FRResult += " " + readResult[i].lines[k].text;

                                    strCurrent = readResult[i].lines[k].text;
                                    strCurrent = strCurrent.Trim();

                                   
                                    if (StringEmptyCheck(strCurrent))
                                    {

                                        if (strCurrent == "*** Totals ***" )
                                        {
                                            if (StringEmptyCheck(temp) && StringEmptyCheck(amount))
                                            {
                                                FRFinalList.Add(new List<string> { initialstr, temp, amount, "", "","" });
                                            }
                                            StartFlag = false;
                                            break;
                                        }
                                        else if ((strCurrent == "Transaction Details" || strCurrent == "PARTICULARS" || strCurrent == "Particulars" || strCurrent == "Narration" || strCurrent == "Transaction Remarks" || strCurrent == "Description") && ContinueWithDateCount)
                                        {
                                            StartFlag = true;
                                            continue;
                                        }
                                        else if (StartFlag != true && IsDateFormatChecker(strCurrent))
                                        {
                                            DateCount++;
                                        }

                                        if (StartFlag != true && DateCount > 5)
                                        {
                                            ContinueWithDateCount = false;
                                            k = 0;
                                        }

                                        if (ContinueWithDateCount == false && IsDateFormatChecker(strCurrent))
                                        {
                                            StartFlag = true;
                                        }

                                        if (StartFlag)
                                        {
                                            if (IsDateFormatChecker(strCurrent))
                                            {
                                                
                                                if (!StringEmptyCheck(initialstr)) // if it is empty then 
                                                    initialstr = strCurrent;

                                                else
                                                {
                                                    temp = temp.Replace("Web Bill Payment - MASTERCARD 9685", "Web Bill Payment - MASTERCARD").Replace("ATM Withdrawal - INTERAC 3990", "ATM Withdrawal - INTERAC").Replace("Interac Purchase - ELECTRONICS 1975", "Interac Purchase - ELECTRONICS").Replace("Web Bill Payment - AMEX 3314", "Web Bill Payment - AMEX");
                                                    temp = temp.Replace("ATM Withdrawal - FIRST BANK 0064", "ATM Withdrawal - FIRST BANK").Replace("Interac Purchase - SUPERMARKET 1559", "Interac Purchase - SUPERMARKET").Replace("Interac Refund - ELECTRONICS 1975", "Interac Refund - ELECTRONICS").Replace("Telephone Bill Payment - VISA 2475", "Telephone Bill Payment - VISA");
                                                    temp = temp.Replace("Web Funds Transfer - From SAVINGS 2620", "Web Funds Transfer - From SAVINGS");

                                                    if (StringEmptyCheck(temp) && StringEmptyCheck(amount))
                                                    {
                                                        FRFinalList.Add(new List<string> { initialstr, temp, amount, "", "","" });
                                                    }

                                                    initialstr = strCurrent;
                                                    temp = "";
                                                    amount = "";
                                                }
                                            }
                                            else
                                            {
                                                if (!IsDigitsCheck(strCurrent))
                                                {
                                                    if (strCurrent == "citibank" || strCurrent == " ICICI Bank "||strCurrent== "Ref. Withdrawals" || strCurrent== "9685"
                                                        || strCurrent== "3990" || strCurrent== "1975" || strCurrent== "3314" || strCurrent== "0064" || strCurrent== "1559"
                                                        || strCurrent== "1975" || strCurrent== "2475" || strCurrent== "2620" || strCurrent== "Deposits" || strCurrent== "Balance")
                                                        // strCurrent == "CASH DEPOSIT" ||
                                                        continue;
                                                    else
                                                    {
                                                        if (!StringEmptyCheck(temp) && strCurrent == "Closing Balance")
                                                            continue;
                                                        else
                                                            temp += strCurrent + " ";
                                                    }
                                                }
                                                else if (!strCurrent.Contains(".") && !strCurrent.Contains(",") && !strCurrent.Contains(".00") && !strCurrent.Contains(".0") && !strCurrent.Contains(',') && !strCurrent.Contains("Dr") && !strCurrent.Contains("Cr"))
                                                {
                                                    temp += strCurrent + " ";
                                                }
                                                else
                                                {
                                                    //if (strCurrent == "247974")
                                                    //    continue;
                                                    //else
                                                    //{
                                                        if (!StringEmptyCheck(amount) && strCurrent != "0.00")
                                                            amount = strCurrent;
                                                    //}
                                                }
                                            }
                                        }
                                    }
                                }
                                //if(StartFlag!=true)
                                //{
                                //    StartFlag = true;
                                //    i -=1;
                                //}
                            }
                        }
                        else
                        {
                            Error = "Err, Can't find the Operation-Location";
                        }
                    }
                    else
                    {
                        Error = "Err, Invalid Response";
                    }
                }
                else if ((int)response.StatusCode == 400)
                {
                    Error = "Err, Bad request";
                }
                else if ((int)response.StatusCode == 404)
                {
                    Error = "Err, Model not found";
                }
                else if ((int)response.StatusCode == 415)
                {
                    Error = "Err, Unsupported media type error. 'Content-Type' does not match the POST content"; 
                }
                else if ((int)response.StatusCode == 500)
                {
                    Error = "Err, Internal server error";
                }
                else if ((int)response.StatusCode == 503)
                {
                    Error = "Err, Transient fault";
                }
                else
                {
                    Error = "Err, Can't access the Form Recognizer Service";
                }
            }
            catch (Exception e)
            {
                Error = "Stack Trace: "+e.StackTrace+" Exception Messgae: " + e.Message;
            }
        }


        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static bool IsDateFormatChecker(string str)
        {
            str = str.Replace(".", "");
            str = str.Replace(",", "").Trim();
            DateTime dDate;
            if (DateTime.TryParse(str, out dDate))
                return true;
            else
                return false;
        }

        private static bool IsDigitsCheck(string str)
        {
            str = str.Replace(" ", "").Replace("-", "");
            foreach (char c in str)
            {
                if (!char.IsDigit(c))
                {
                    if (c == ',' || c == '.' || c == 'C' || c == 'c' || c == 'r')
                        continue;
                    return false;
                }
            }
            return true;
        }

        //private static string IsDigitsOnly(string str)
        //{
        //    str = str.Replace(" ", "").Replace("-","");
        //    foreach (char c in str)
        //    {
        //        if (c < '0' || c > '9')
        //        {
        //            if (c == ',' || c == '.' || c == 'C' || c == 'c' || c == 'r')
        //                continue;
        //         return "-1";
        //        }
        //    }
        //    return str;
        //}

        //private static string IsAmountFieldCheck(string str)
        //{
        //    str = str.Replace(" ", "").Replace("-", "");
        //    foreach (char c in str)
        //    {
        //        if ((c < '0' || c > '9') && (str.Contains(".00") || !str.Contains(".0") || !str.Contains(',')))
        //        {
        //            continue;
        //        }
        //        else
        //            return "-1";
        //    }
        //    return str;
        //}

        private static bool IsQTACheck(string str)
        {
            // Create  a string array and add the special characters you want to remove
            string[] chars = new string[] { " ", "+", "-", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]", "=", "?" };
            //Iterate the number of times based on the String array length.
            for (int i = 0; i < chars.Length; i++)
            {
                if (str.Contains(chars[i]))
                {
                    str = str.Replace(chars[i], "");
                }
            }
            return (str.ToLower() == "") ? true : false;
        }

        private static bool StringEmptyCheck(string str)
        {
            return (str != null && str != String.Empty) ? true : false;
        }
    }
}