using TalentProWebApp.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Newtonsoft.Json;
using NLog;

namespace TalentProWebApp
{
    

    public static class GetAzureSearchResults
    {
        private static readonly string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
        private static readonly string APIKey = ConfigurationManager.AppSettings["api-Key"];
        private static readonly string docIndex = ConfigurationManager.AppSettings["docIndex"];
        private static readonly string videoIndex = ConfigurationManager.AppSettings["videoIndex"];
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();


        public static async Task<AzSearchResultModel<T>> GetDocListAsync<T>(AzSearchModel azSearch) where T : IAzResutValueModel
        {
            LOGGER.Info("Calling Azure Search API started ");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept","application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            client.DefaultRequestHeaders.TryAddWithoutValidation("api-key", APIKey);

            string apirURL = "";
            if (typeof(T) == typeof(DocumentModel))
            {
                apirURL = BaseURL + docIndex;
            }
            else
            {
                apirURL = BaseURL + videoIndex;
            }
            LOGGER.Info("Calling Azure Search API parameters apirurl" +apirURL+" parameters "+ Dump(azSearch));
            HttpResponseMessage response = await client.PostAsJsonAsync(apirURL, azSearch);
            LOGGER.Info("Response from Azure Search API parameters apirurl" + apirURL + " parameters " + Dump(azSearch)+ " is "+response.StatusCode);
            response.EnsureSuccessStatusCode();                       

            var results= await response.Content.ReadAsAsync<AzSearchResultModel<T>>();
            LOGGER.Info("Calling Azure Search API Completed successfully ");
            return results;
        }
        public static string Dump(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

 
}