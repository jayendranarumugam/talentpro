using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;


namespace TalentProWebApp
{
    public sealed class AzureBlob
    {
        private static readonly string StorageConStr = ConfigurationManager.AppSettings["StorageConnStr"];


        //public CloudStorageAccount StorageAccount { get; private set; }
        //public CloudBlobClient BlobClient { get; private set; }
        //public CloudBlobContainer Container { get; private set; }

        public static CloudBlobContainer AzureBlobContainer(string ContainerName)
        {
            CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(StorageConStr);
            //instantiate the client
            CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
            //set the container
            CloudBlobContainer Container = BlobClient.GetContainerReference(ContainerName);

            return Container;

        }

        //public static AzureBlob Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            lock (syncRoot)
        //            {
        //                if (instance == null)
        //                    instance = new AzureBlob(string ContainerName);
        //            }
        //        }

        //        return instance;
        //    }
    }


}
