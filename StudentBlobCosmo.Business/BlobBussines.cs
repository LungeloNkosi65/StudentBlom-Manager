using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace StudentBloCosmo.Service
{
    public class BlobBussines
    {
        private CloudBlobContainer blobContainer;

        public BlobBussines(string ContainerName)
        {
            // Check if Container Name is null or empty
            if (string.IsNullOrEmpty(ContainerName))
            {
                throw new ArgumentNullException("ContainerName", "Container Name can't be empty");
            }
            try
            {
                // Get azure table storage connection string.
                string ConnectionString = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = cloudBlobClient.GetContainerReference(ContainerName);

                // Create the container and set the permission
                if (blobContainer.CreateIfNotExists())
                {
                    blobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        }
                    );
                }
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public string UploadFile(HttpPostedFileBase FileToUpload)
        {
            string AbsoluteUri;
            // Check HttpPostedFileBase is null or not
            if (FileToUpload == null || FileToUpload.ContentLength == 0)
                return null;
            try
            {
                string FileName = Path.GetFileName(FileToUpload.FileName);
                CloudBlockBlob blockBlob;
                // Create a block blob
                blockBlob = blobContainer.GetBlockBlobReference(FileName);
                // Set the object's content type
                blockBlob.Properties.ContentType = FileToUpload.ContentType;
                // upload to blob
                blockBlob.UploadFromStream(FileToUpload.InputStream);

                // get file uri
                AbsoluteUri = blockBlob.Uri.AbsoluteUri;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
            return AbsoluteUri;
        }
        public bool DeleteBlob(string AbsoluteUri)
        {
            try
            {
                Uri uriObj = new Uri(AbsoluteUri);
                string BlobName = Path.GetFileName(uriObj.LocalPath);

                // get block blob refarence
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(BlobName);

                // delete blob from container    
                blockBlob.Delete();
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        public List<string> BlobList()
        {
            List<string> _blobList = new List<string>();
            foreach (IListBlobItem item in blobContainer.ListBlobs())
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob _blobpage = (CloudBlockBlob)item;
                    _blobList.Add(_blobpage.Uri.AbsoluteUri.ToString());
                }
            }
            return _blobList;
        }
    }
}
