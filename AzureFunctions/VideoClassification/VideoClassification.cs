using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace VideoClassification
{
    public static class VideoClassification
    {

        private static string[] _validExtensions = { "mp4" };

        private static string apiUrl = Environment.GetEnvironmentVariable("apiUrl");
        private static string location = Environment.GetEnvironmentVariable("apiLocation");
        private static string apiKey = Environment.GetEnvironmentVariable("apiKey");

        //private static string key = TelemetryConfiguration.Active.InstrumentationKey = "a6468eb4-f179-4064-a6cc-018b77976040";

        //private static TelemetryClient telemetryClient = new TelemetryClient() { InstrumentationKey = key };

        [FunctionName("VideoClassificationFunction")]
        public static async Task RunAsync([BlobTrigger("videocontainer/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob myBlob, string name, ILogger log)
        {

            System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;

            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.StreamWriteSizeInBytes} Bytes");

            string[] spiltfilenamewithext = name.Split('.');
            //string filenamewithoutextension = spiltfilenamewithext[0];

            if (IsValidExtension(spiltfilenamewithext[spiltfilenamewithext.Length - 1]))
            {
                var handler = new HttpClientHandler();
                handler.AllowAutoRedirect = false;
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);

                string queryParams = CreateQueryString(new Dictionary<string, string>()
                {
                    {"generateAccessTokens", "true"},
                    {"allowEdit", "true"},
                });

                HttpResponseMessage result = await client.GetAsync($"{apiUrl}/auth/trial/Accounts?{queryParams}");
                var json = await result.Content.ReadAsStringAsync();
                var accounts = JsonConvert.DeserializeObject<AccountContractSlim[]>(json);
                // take the relevant account, here we simply take the first
                var accountInfo = accounts.First();

                // we will use the access token from here on, no need for the apim key
                client.DefaultRequestHeaders.Remove("Ocp-Apim-Subscription-Key");

                // upload a video
                var content = new MultipartFormDataContent();
                log.LogInformation("Uploading...");

                // as an alternative to specifying video URL, you can upload a file.
                // remove the videoUrl parameter from the query params below and add the following lines:
                var sasToken = myBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(30),//assuming the blob can be downloaded in 30 minutes
                });

                var videoUrl = string.Format("{0}{1}", myBlob.Uri, sasToken);

                queryParams = CreateQueryString(
                    new Dictionary<string, string>()
                    {
            {"accessToken", accountInfo.AccessToken},
            {"name", name},
            {"description", "video_description"},
            {"privacy", "private"},
            {"partition", "ResumeVideo"},
            {"videourl", videoUrl}
                    });

                var uploadRequestResult = await client.PostAsync($"{apiUrl}/{accountInfo.Location}/Accounts/{accountInfo.Id}/Videos?{queryParams}", content);
                var uploadResult = await uploadRequestResult.Content.ReadAsStringAsync();


                if (uploadRequestResult.IsSuccessStatusCode)
                {
                    // get the video ID from the upload result
                    string videoId = JsonConvert.DeserializeObject<dynamic>(uploadResult)["id"];
                    log.LogInformation("Uploaded");
                    log.LogInformation("Video ID:" + videoId);



                    // wait for the video index to finish
                    while (true)
                    {
                        await Task.Delay(10000);

                        queryParams = CreateQueryString(
                            new Dictionary<string, string>()
                            {
                {"accessToken", accountInfo.AccessToken},
                {"language", "English"},
                            });

                        var videoGetIndexRequestResult = await client.GetAsync($"{apiUrl}/{accountInfo.Location}/Accounts/{accountInfo.Id}/Videos/{videoId}/Index?{queryParams}");


                        var videoGetIndexResult = await videoGetIndexRequestResult.Content.ReadAsAsync<VideoDocModel>();



                        string processingState = videoGetIndexResult.State;

                        log.LogInformation("State is : " + processingState + "for Video ID: " + videoId);

                        // job is finished
                        if (processingState != "Uploaded" && processingState != "Processing")
                        {
                            try
                            {
                                log.LogInformation("Starting saving into Cosmos DB for the Video ID:" + videoId);
                                CosmosDB cosmosDB = new CosmosDB();
                                cosmosDB.CreateDocumentDB().Wait();
                                await cosmosDB.UpdInsResumeDocumentAsync(videoGetIndexResult);
                                log.LogInformation("Sucessfully saved into Cosmos DB for the Video ID:" + videoId);
                                break;
                            }
                            catch (Exception e)
                            {
                                log.LogError(e.Message.ToString() + "for the videoID" + videoId);
                            }

                        }
                    }

                }
                else
                {
                    log.LogError(uploadResult + " for the filename" + name);
                }

            }



        }
        public static bool IsValidExtension(string ext)
        {
            return _validExtensions.Contains(ext.ToLower());
        }

        private static string CreateQueryString(IDictionary<string, string> parameters)
        {
            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in parameters)
            {
                queryParameters[parameter.Key] = parameter.Value;
            }

            return queryParameters.ToString();
        }


    }

    public class AccountContractSlim
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string AccountType { get; set; }
        public string Url { get; set; }
        public string AccessToken { get; set; }
    }
}
