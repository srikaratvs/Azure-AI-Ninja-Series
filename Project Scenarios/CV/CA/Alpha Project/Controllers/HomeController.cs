using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Alpha_Project.BlobStorage;

namespace Alpha_Project.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index()
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

        [HttpPost]
        public ActionResult UploadFiles()
        {
            string file_extension=null;
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";
                        //string filename = Path.GetFileName(Request.Files[i].FileName);

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                            file_extension = fname.Substring(fname.LastIndexOf(".") + 1);

                        }

                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                        {
                            fileData = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                        }

                        try
                        {
                            //string imagestr = Convert.ToBase64String(fileData); //Image storing in Blob Account
                            //StoreBlob.StoreImageInBlob(imagestr, file_extension);

                            FormRecognizer fr = new FormRecognizer();

                            fr.GenerateText(fileData); // form recognizer calling
                            fr.DoLuis(); //luis calling 

                            //////Open file for Read\Write
                            //FileStream fs = new FileStream(@"E:\Microsoft Demo 26-Nov-2020\CA\Data\Sample.txt", FileMode.Append, FileAccess.Write);
                            //StreamWriter sw = new StreamWriter(fs);
                            ////Create StreamWriter object to write string to FileSream
                            //for (int j = 0; j < fr.FRFinalList.Count; j++)
                            //{
                            //    sw.WriteLine("Date" + " " + fr.FRFinalList[j][0] + " " + "Narration" + " " + fr.FRFinalList[j][1] + " " + "Amount" + " " + fr.FRFinalList[j][2] + " " + "TType" + " " + fr.FRFinalList[j][3] + " " + "Class" + " " + fr.FRFinalList[j][4] + " " + "SubClass" + " " + fr.FRFinalList[j][5] + "\n\n");
                            //}

                            //sw.Close();

                            if (fr.Error == "")
                            {
                                return Json(new { StatusCode = "S", FullJSON = fr.FRFinalList});
                            }
                            
                            return Json(new { StatusCode = "F", Message = fr.Error });
                        }
                        catch (Exception e)// handling runtime errors and returning error as Json
                        {
                            return Json(new { StatusCode = "F", Message = e.Message });
                        }
                        //string img = Convert.ToBase64String(fileData);

                        // Get the complete folder path and store the file inside it.  
                        //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        //file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
    }
}
