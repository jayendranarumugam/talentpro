using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VideoClassification
{
    public class CosmosDB
    {
        private static string EndpointUri = Environment.GetEnvironmentVariable("CosmosEndoint");
        private static string PrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey");
        private static string DatabaseId = Environment.GetEnvironmentVariable("CosmosDatabase");
        private static string CollectionId = Environment.GetEnvironmentVariable("CosmosCollection");
        private DocumentClient client;

        public async Task CreateDocumentDB()
        {
            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseId), new DocumentCollection { Id = CollectionId });
        }


        public async Task UpdInsResumeDocumentAsync(VideoDocModel videoDoc)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true };

            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), videoDoc);


        }

    }
}
