using System;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace AnnotationPoC.Controllers
{
    public class AnnotationController : Controller
    {
        // GET: Annotation Index View
        public ActionResult Index()
        {
            return View();
        }

        // POST: Image Annotation
        [HttpPost]
        public async Task<JsonResult> ImageAnnotation(string InImage,string OutImage)
        {
            try
            {
                IndoorImageAnnotation iia = new IndoorImageAnnotation(); //Creating object for IndoorImageAnnotation class
                iia.DeepImageAnnotation(InImage); // doing Deep Image classification
                if(iia.Error=="") //Won't allow to do Outdoor Image Annotation if any error occur in Indoor Image Annotation
                {
                    OutdoorImageAnnotation oia = new OutdoorImageAnnotation(); //Creating object for OcrImageAnnotation class
                    await oia.OcrImageAnnotation(OutImage); // doing Ocr and Luis
                    return Json(new { Result = iia.Result, Probability = iia.Probability,StoreType=oia.StoreType, Error = oia.Error, OCRText=oia.OcrResult }); //Returnning indoor and outdoor image annotaition result

                }
                else // return error message, which is occur in Indoor Image Annotation
                    return Json(new { Result = iia.Result,Probability=iia.Probability, StoreType ="", Error = iia.Error });

            }
            catch (Exception e)// handling runtime errors and returning error as Json
            {
                return Json(new { Result = "", Probability = 0, StoreType = "", Error = e.Message });
            }
        }
    }
}