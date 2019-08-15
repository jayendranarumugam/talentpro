using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DocumentClassification
{
    public class CosmosDB
    {
        private static string EndpointUri = Environment.GetEnvironmentVariable("CosmosEndoint");
        private static string PrimaryKey = Environment.GetEnvironmentVariable("CosmosPrimaryKey");
        private static string DatabaseId =Environment.GetEnvironmentVariable("CosmosDatabase"); 
        private static string CollectionId =Environment.GetEnvironmentVariable("CosmosCollection"); 
        private DocumentClient client;


        public async Task CreateDocumentDB()
        {
            this.client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseId), new DocumentCollection { Id = CollectionId });
        }


        public async Task UpdInsResumeDocumentAsync(ResumeDocModel resumeDoc)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = 1,EnableCrossPartitionQuery=true};

            List<ResumeDocModel> resumeQueryResult = client.CreateDocumentQuery<ResumeDocModel>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), 
                   "SELECT * FROM Resume WHERE Resume.DocumentName='" + resumeDoc.DocumentName+"'", queryOptions).ToList();

            if (resumeQueryResult.Count() == 0)
            {
                await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), resumeDoc);
            }          
            else
            {
                resumeDoc.id=resumeQueryResult.First().id;
                await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DatabaseId, CollectionId, resumeQueryResult.First().id), resumeDoc);
            }                     

        }

    }
}
