using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace TalentProWebApp
{
    public static class DocumentDBRepository<T> where T : class
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        private static DocumentClient client;

        public static void Initialize()
        {
            client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["endpoint"]), ConfigurationManager.AppSettings["authKey"]);
        }

        public static async Task<(IEnumerable<T>,string)> GetItemsAsync(int pageSize,string continuationToken)
        {
            FeedOptions queryOptions = new FeedOptions {  MaxItemCount= pageSize,RequestContinuation= continuationToken };

            IDocumentQuery<T> query = client.CreateDocumentQuery<T>(
                UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), "SELECT * FROM Resume", queryOptions)
                .AsDocumentQuery();

            List<T> results = new List<T>();
            string continationToken=null;
            int pageCounter = 0;

            while (query.HasMoreResults && pageCounter < pageSize)
            {
                var result = await query.ExecuteNextAsync<T>();
                continationToken = result.ResponseContinuation;
                results.AddRange(result);
                pageCounter++;
            }
            
            return (results, continationToken);
        }
    }
}