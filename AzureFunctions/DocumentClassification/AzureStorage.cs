using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentClassification
{
    public sealed class AzureStorage
    {
        private static volatile AzureStorage instance;
        private static object syncRoot = new Object();
        private static string ImagecontainerName = Environment.GetEnvironmentVariable("ContainerNameToUploadImages");

        public CloudStorageAccount StorageAccount { get; private set; }
        public CloudBlobClient BlobClient { get; private set; }
        public CloudBlobContainer Container { get; private set; }

        private AzureStorage()
        {
            StorageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));

            //instantiate the client
            BlobClient = StorageAccount.CreateCloudBlobClient();

            //set the container
            Container = BlobClient.GetContainerReference(ImagecontainerName);

        }

        public static AzureStorage Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AzureStorage();
                    }
                }

                return instance;
            }
        }
    }

    public class AzureStorageModel
    {     
        public List<IListBlobItem> IblobList { get; set; }
        public CloudBlobContainer Container { get; set; }
    }

    public class AzStorage
    {
        public static AzureStorageModel GetBlobDirectoryList(string directoryName)
        {
            AzureStorage storageClient = AzureStorage.Instance;
            AzureStorageModel azureStorageModel = new AzureStorageModel();
            CloudBlobDirectory directory = storageClient.Container.GetDirectoryReference(directoryName);
            azureStorageModel.Container = storageClient.Container;
            azureStorageModel.IblobList = directory.ListBlobs().ToList();
            return azureStorageModel;
        }
        public static string GetBlobSasUri(CloudBlockBlob cloudBlob)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-5);
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(5);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            //Generate the shared access signature on the blob, setting the constraints directly on the signature.
            string sasBlobToken = cloudBlob.GetSharedAccessSignature(sasConstraints);

            //Return the URI string for the container, including the SAS token.
            return cloudBlob.Uri + sasBlobToken;
        }
    }

}
