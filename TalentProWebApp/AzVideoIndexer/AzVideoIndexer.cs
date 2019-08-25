using TalentProWebApp.Models;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;


namespace TalentProWebApp
{
    public class AzVideoIndexer
    {
        private static readonly string VideoIndexerBaseURL = ConfigurationManager.AppSettings["videoIndexBaseURL"];
        private static readonly string APIKey = ConfigurationManager.AppSettings["videoIndexApiKey"];

        

        public static EmbedVideoIndexModel GetEmbeddedDetailsAsync(VideoModel videoModel)
        {
            EmbedVideoIndexModel embedVideoIndex = new EmbedVideoIndexModel();
            string token = GetAccessTokenAsync(videoModel);
            embedVideoIndex.playerURL = "https://www.videoindexer.ai/embed/player/" + videoModel.accountId + "/" + videoModel.id + "?accessToken=" + token;
            embedVideoIndex.InsightURL = "https://www.videoindexer.ai/embed/insights/" + videoModel.accountId + "/" + videoModel.id + "?accessToken=" + token;

            return embedVideoIndex;
        }


        public  static string GetAccessTokenAsync(VideoModel videoModel)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.ServicePointManager.SecurityProtocol | System.Net.SecurityProtocolType.Tls12;

            string accessToken = "";
            HttpResponseMessage result;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", APIKey);

                string url = VideoIndexerBaseURL + videoModel.accountId + "/Videos/" + videoModel.id + "/AccessToken?allowEdit=false";
                Task task = Task.Run(async () =>
                {
                    result = await client.GetAsync(url);
                    if (result.IsSuccessStatusCode)
                    {
                        accessToken = await result.Content.ReadAsStringAsync();
                        accessToken = accessToken.Replace("\"", "");
                    }
                });
                task.Wait();
            }
            return accessToken;
        }
    }


}