using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.WindowsAzure.Storage.Auth;
//using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace Alpha_Project
{
    public class BlobStorage
    {
        public class StoreBlob
        {
            private static string AcccountName = ConfigurationManager.AppSettings["BlobStorage_AccName"], containername = ConfigurationManager.AppSettings["BlobStorage_ConName"], BlobKey = ConfigurationManager.AppSettings["BlobStorage_Key"];
            public string error = "";

            public static string StoreImageInBlob(string data, string extension)
            {
                //return new AzureBlobStorage(AcccountName, BlobKey, containername).StoreTextInPrivateBlob(data);

                CloudBlobContainer cont = new CloudStorageAccount(new StorageCredentials(AcccountName, BlobKey), useHttps: true).CreateCloudBlobClient().GetContainerReference(containername);
                cont.CreateIfNotExists();
                cont.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                CloudBlockBlob cblob = cont.GetBlockBlobReference("Statement_" + DateTime.Now.ToString("ddMMyyyy_HHmmss_ffffff") + "."+ extension);//name should be unique otherwise override at same name.  

                byte[] imageBytes = Convert.FromBase64String(data);
                cblob.UploadFromStream(new MemoryStream(imageBytes));
                return cblob.Uri.AbsoluteUri;
            }
        }
    }
}