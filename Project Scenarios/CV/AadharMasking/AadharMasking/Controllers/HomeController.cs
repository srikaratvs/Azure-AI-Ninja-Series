using AzureOCR;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Drawing; //install System.Drawing.Common
using System.IO;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Web.Services;
using System.Web.Script.Serialization;
//using System.Windows;
//using System.Windows.Media;
//install System.Drawing.Common

namespace AadharMasking.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Masking()
        {
            return View();
        }




        public JsonResult LoginCheck(string Login, string Password)
        {
            try
            {               

                Session["UserId"] = Login;

                if (Login == "Admin" && Password == "JSN123!")
                {

                    return Json(new { StatusCode = "200", Message = "Login Successful" });
                }
                
                return Json(new { StatusCode = "500", Message = "Invalid Username and Password" });
            }
            catch (Exception e)// handling runtime errors and returning error as Json
            {
                return Json(new { StatusCode = "500", Message = e.Message });
            }
        }

        //Logout Check Function
        public JsonResult LogoutCheck()
        {
            try
            {
                Session.Clear();
                Session.Abandon();

                return Json(new { StatusCode = "200" });
            }
            catch (Exception e)
            {
                return Json(new { StatusCode = "500", Message = e.Message });
            }
        }

        private static Bitmap RotateImageCut(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }
        //private static Bitmap RotateImageWithoutCut(Bitmap bmp, float angle)
        //{
        //    float height = bmp.Height;
        //    float width = bmp.Width;
        //    int hypotenuse = System.Convert.ToInt32(System.Math.Floor(Math.Sqrt(height * height + width * width)));
        //    Bitmap rotatedImage = new Bitmap(hypotenuse, hypotenuse);
        //    using (Graphics g = Graphics.FromImage(rotatedImage))
        //    {
        //        g.TranslateTransform((float)rotatedImage.Width / 2, (float)rotatedImage.Height / 2); //set the rotation point as the center into the matrix
        //        g.RotateTransform(angle); //rotate
        //        g.TranslateTransform(-(float)rotatedImage.Width / 2, -(float)rotatedImage.Height / 2); //restore rotation point into the matrix
        //        g.DrawImage(bmp, (hypotenuse - width) / 2, (hypotenuse - height) / 2, width, height);
        //    }


        //    int lr = (rotatedImage.Width - bmp.Width)/2;
        //    int tb= (rotatedImage.Height - bmp.Height) / 2;

        //    Bitmap target = new Bitmap(bmp.Width, bmp.Height);
        //    Rectangle cropRect = new Rectangle(lr,tb, target.Width, target.Height);
        //    using (Graphics g = Graphics.FromImage(target))
        //    {
        //        g.DrawImage(rotatedImage, new Rectangle(0, 0, target.Width, target.Height),
        //                         cropRect,
        //                         GraphicsUnit.Pixel);
        //    }


        //    return target;
        //}

        //private static Bitmap RotateImageWithoutCut1(Bitmap bmp, float angle)
        //{
        //    float height = bmp.Height;
        //    float width = bmp.Width;
        //    int hypotenuse = System.Convert.ToInt32(System.Math.Floor(Math.Sqrt(height * height + width * width)));
        //    Bitmap rotatedImage = new Bitmap(hypotenuse, hypotenuse);
        //    using (Graphics g = Graphics.FromImage(rotatedImage))
        //    {
        //        g.TranslateTransform((float)rotatedImage.Width / 2, (float)rotatedImage.Height / 2); //set the rotation point as the center into the matrix
        //        g.RotateTransform(angle); //rotate
        //        g.TranslateTransform(-(float)rotatedImage.Width / 2, -(float)rotatedImage.Height / 2); //restore rotation point into the matrix
        //        g.DrawImage(bmp, (hypotenuse - width) / 2, (hypotenuse - height) / 2, width, height);
        //    }
        //    return rotatedImage;
        //}

        //private static Bitmap RotateImageWithoutCut(Bitmap bmp, float angle)
        //{
        //    float alpha = angle;

        //    //edit: negative angle +360
        //    while (alpha < 0) alpha += 360;

        //    float gamma = 90;
        //    float beta = 180 - angle - gamma;

        //    float c1 = bmp.Height;
        //    float a1 = (float)(c1 * Math.Sin(alpha * Math.PI / 180) / Math.Sin(gamma * Math.PI / 180));
        //    float b1 = (float)(c1 * Math.Sin(beta * Math.PI / 180) / Math.Sin(gamma * Math.PI / 180));

        //    float c2 = bmp.Width;
        //    float a2 = (float)(c2 * Math.Sin(alpha * Math.PI / 180) / Math.Sin(gamma * Math.PI / 180));
        //    float b2 = (float)(c2 * Math.Sin(beta * Math.PI / 180) / Math.Sin(gamma * Math.PI / 180));

        //    int width = Convert.ToInt32(b2 + a1);
        //    int height = Convert.ToInt32(b1 + a2);

        //    Bitmap rotatedImage = new Bitmap(width, height);
        //    using (Graphics g = Graphics.FromImage(rotatedImage))
        //    {
        //        g.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2); //set the rotation point as the center into the matrix
        //        g.RotateTransform(angle); //rotate
        //        g.TranslateTransform(-rotatedImage.Width / 2, -rotatedImage.Height / 2); //restore rotation point into the matrix
        //        g.DrawImage(bmp, new Point((width - bmp.Width) / 2, (height - bmp.Height) / 2)); //draw the image on the new bitmap
        //    }
        //    return rotatedImage;
        //}



        public static Bitmap ChangeColor(Bitmap scrBitmap, int x,int y,int w,int h)
        {
            //You can change your new color here. Red,Green,LawnGreen any..
            Color newColor = Color.Green;
            //Color actualColor;
            //make an empty bitmap the same size as scrBitmap
            //Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            //int x = 764;
            //int y = 1079;
            //int w = 1317;
            //int h = 1149;

            //int x = 50;
            //int y = 10;
            //int w = 200;
            //int h = 400;
            for (int i = x; i < w; i++)
            {
                for (int j = y; j < h; j++)
                {                   
                    scrBitmap.SetPixel(i, j, newColor);
                }
            }
            return scrBitmap;
        }

        public static byte[] ImageToByte2(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }

        public string ImageToBase64(Image image,  System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }



        public Image OrientationImage(Bitmap bm, int orientation)
        {
            using (Graphics gr = Graphics.FromImage(bm))
            {

                // Orient the result.
                switch (orientation)
                {
                    case 1:
                        break;
                    case 2:
                        gr.ScaleTransform(-1, 1);
                        break;
                    case 3:
                        gr.RotateTransform(180);
                        break;
                    case 4:
                        gr.ScaleTransform(1, -1);
                        break;
                    case 5:
                        gr.RotateTransform(90);
                        gr.ScaleTransform(-1, 1, MatrixOrder.Append);
                        break;
                    case 6:
                        gr.RotateTransform(-90);
                        break;
                    case 7:
                        gr.RotateTransform(90);
                        gr.ScaleTransform(1, -1, MatrixOrder.Append);
                        break;
                    case 8:
                        gr.RotateTransform(90);
                        break;
                }
            }
            return bm;
        }
        public enum ExifOrientations
        {
            Unknown = 0,
            TopLeft = 1,
            TopRight = 2,
            BottomRight = 3,
            BottomLeft = 4,
            LeftTop = 5,
            RightTop = 6,
            RightBottom = 7,
            LeftBottom = 8,
        }

        public Image OrientationImage(Bitmap bm,ExifOrientations orientation)
        {
            using (Graphics gr = Graphics.FromImage(bm))
            {
                
                // Orient the result.
                switch (orientation)
                {
                    case ExifOrientations.TopLeft:
                        break;
                    case ExifOrientations.TopRight:
                        gr.ScaleTransform(-1, 1);
                        break;
                    case ExifOrientations.BottomRight:
                        gr.RotateTransform(180);
                        break;
                    case ExifOrientations.BottomLeft:
                        gr.ScaleTransform(1, -1);
                        break;
                    case ExifOrientations.LeftTop:
                        gr.RotateTransform(90);
                        gr.ScaleTransform(-1, 1, MatrixOrder.Append);
                        break;
                    case ExifOrientations.RightTop:
                        gr.RotateTransform(-90);
                        break;
                    case ExifOrientations.RightBottom:
                        gr.RotateTransform(90);
                        gr.ScaleTransform(1, -1, MatrixOrder.Append);
                        break;
                    case ExifOrientations.LeftBottom:
                        gr.RotateTransform(90);
                        break;
                }
            }
            return bm;
        }



        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        [HttpPost]
        public void SaveImage(string ImageData)
        {
            if (string.IsNullOrEmpty(ImageData))
                return;

            var t = ImageData.Substring(22);  // remove data:image/png;base64,

            byte[] bytes = Convert.FromBase64String(t);

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            var randomFileName = Guid.NewGuid().ToString().Substring(0, 4) + ".png";
            var fullPath = Path.Combine(Server.MapPath("~/Content/Images/"), randomFileName);
            image.Save(fullPath, System.Drawing.Imaging.ImageFormat.Png);
        }

        //private void bBoxRotate(int x,int y, int cx,int cy, float angle)
        //{
        //    // cx, cy - center of square coordinates
        //    // x, y - coordinates of a corner point of the square
        //    // theta is the angle of rotation

        //    // translate point to origin
        //    float tempX = x - cx;
        //    float tempY = y - cy;

        //    // now apply rotation
        //    float rotatedX = tempX * cos(angle) - tempY * sin(angle);
        //    float rotatedY = tempX * sin(angle) + tempY * cos(angle);

        //    // translate back
        //    x = (int)rotatedX + cx;
        //    y = (int)rotatedY + cy;
        //}

        /// <summary>
        /// Rotates the specified point around another center.
        /// </summary>
        /// <param name="center">Center point to rotate around.</param>
        /// <param name="pt">Point to rotate.</param>
        /// <param name="degree">Rotation degree. A value between 1 to 360.</param>
        public static Point RotatePoint(Point center, Point pt, float degree)
        {
            double x1, x2, y1, y2;
            x1 = center.X;
            y1 = center.Y;
            x2 = pt.X;
            y2 = pt.Y;
            double distance = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            degree *= (float)(Math.PI / 180);
            double x3, y3;
            x3 = distance * Math.Cos(degree) + x1;
            y3 = distance * Math.Sin(degree) + y1;
            return new Point((int)x3, (int)y3);
        }

        //private static void rotpoint()
        //{
            
        //    var originalPoint = new Vector(10, 0);
        //    var transform = Matrix.Identity;
        //    transform.Rotate(45.0); // 45 degree rotation
        //    var rotatedPoint = originalPoint * transform;
        //}

        //POST: Recognize Text
        [HttpPost]
        public async Task<JsonResult> TataAIGVision(string ImageData)
        {
            try
            {
            AzurImageOCR ai = new AzurImageOCR();
            await ai.OcrImage(ImageData);
                if (ai.Error == "")
                {
                    List<IList<int>> OCRBoxList = ai.OCRBoxList;
                    if(OCRBoxList.Count==1)
                    {
                        IList<int> re = OCRBoxList[0];
                        //Point cen=new Point((int)ai.Width/2,(int)ai.Height/2);
                        //Point p1 = RotatePoint(cen, new Point(re[0], re[1]), ai.Angle);
                        //Point p2 = RotatePoint(cen, new Point(re[2], re[3]), ai.Angle);
                        //Point p3 = RotatePoint(cen, new Point(re[4], re[5]), ai.Angle);
                        //Point p4 = RotatePoint(cen, new Point(re[6], re[7]), ai.Angle);

                        //int x = (p1.X + p4.X) / 2;
                        //int y = (p1.Y + p2.Y) / 2;
                        //int w = (p2.X + p3.X) / 2;
                        //int h = (p3.Y + p4.Y) / 2;

                        //int x = (int)(re[0] * Math.Cos(ai.Angle));
                        //int y = (int)(re[1] * Math.Sin(ai.Angle));
                        //int w = (int)(re[2] * Math.Sin(ai.Angle)); 
                        //int h = (int)(re[5] * Math.Sin(ai.Angle));

                        //x = (int)((re[0] * Math.Cos(ai.Angle)) - (re[1] * Math.Sin(ai.Angle)));
                        //y = (int)((re[0] * Math.Sin(ai.Angle)) + (re[1] * Math.Cos(ai.Angle)));

                        //x2 = (positionX + width / 2 * Math.cos(rotation)) - (positionY + height / 2 * Math.sin(rotation));
                        //y2 = (positionX + width / 2 * Math.sin(rotation)) + (positionY + height / 2 * Math.cos(rotation));



                        //Matrix transformMatrix = new Matrix();
                        //transformMatrix.RotateAt(ai.Angle, new Point(re[0], re[1]));
                        //float[] val=transformMatrix.Elements;

                        //int x = (re[0] + re[6]) / 2;
                        //int y = (re[1] + re[3]) / 2;
                        //int w = (re[2] + re[4]) / 2;
                        //int h = (re[5] + re[7]) / 2;
                        Bitmap bmp = null;
                        float angle = 360 - ai.Angle;
                        bmp = (Bitmap)Base64ToImage(ImageData);
                        bmp = RotateImageCut(bmp, angle);
                        ai = new AzurImageOCR();
                        await ai.OcrImage(ImageToBase64(bmp, ImageFormat.Jpeg));
                        if (ai.Error == "")
                        {
                            OCRBoxList = ai.OCRBoxList;
                            if (OCRBoxList.Count == 1)
                            {
                                re = OCRBoxList[0];
                                int x = Math.Min(re[0] , re[6]);
                                int y = Math.Min(re[1] , re[3]);
                                int w = Math.Max(re[2] , re[4]);
                                int h = Math.Max(re[5] , re[7]);

                                //bmp = (Bitmap)OrientationImage(bmp, Angle);
                                bmp = ChangeColor(bmp, x, y, w, h);
                                return Json(new { StatusCode = "200", Message = "data:image/jpg;base64," + ImageToBase64(bmp, ImageFormat.Jpeg) });
                            }
                            else
                                return Json(new { StatusCode = "400", Message = "Could not find Aadhar number" });
                            
                        }
                        else                        
                            return Json(new { StatusCode = "400", Message = ai.Error });
                    }
                    else
                        return Json(new { StatusCode = "400", Message = "Could not find Aadhar number" });                
                }
                else                
                    return Json(new { StatusCode = "400", Message = ai.Error });                
            }
            catch (Exception e)// handling runtime errors and returning error as Json
            {
                return Json(new { StatusCode = "400", Message = e.Message });
            }
        }
    }

    //public struct Point
    //{
    //    public Point(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }
    //    public int X { get; set; }
    //    public int Y { get; set; }
    //}
}