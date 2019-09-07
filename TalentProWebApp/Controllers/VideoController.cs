using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NLog;
using TalentProWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace TalentProWebApp.Controllers
{
    [Authorize]
    public class VideoController : Controller
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

        private readonly int pageSize = Convert.ToInt16(ConfigurationManager.AppSettings["PageSizeForVideo"]);
        private readonly string VideoContainerName = ConfigurationManager.AppSettings["VideoContainerName"].ToString();
        private static SearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        private static string IndexName = ConfigurationManager.AppSettings["VideoIndexName"];

        private void InitSearch()
        {
            string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
            string apiKey = ConfigurationManager.AppSettings["api-Key"];

            // Create a reference to the NYCJobs index
            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _searchClient.Indexes.GetClient(IndexName);
        }


        public ActionResult Index()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();

            LOGGER.Info("Index Page called by " + email);
            return View();
        }


        [ActionName("Search")]
        public async Task<ActionResult> Search(string search, int pageNo = 0, VideoType videoType = VideoType.Resume)
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();
            LOGGER.Info("Search video link called by " + email + "parameters are " + search + "page number " + pageNo);

            AzSearchModel azSearchModel = new AzSearchModel { search = search, skip = pageNo * pageSize, top = pageSize };

            var items = await GetAzureSearchResults.GetDocListAsync<VideoModel>(azSearchModel);
            ViewData["VideopageNo"] = pageNo;
            ViewData["VideopageSize"] = pageSize;
            ViewData["VideototalRemainingPageSize"] = (int)Math.Ceiling((double)items.odatacount / pageSize) - pageNo;

            LOGGER.Info("Search video link called by " + email + "parameters are " + search + "page number " + pageNo + "completed sucessfully");
            return View(items.value);

        }

        public ActionResult UploadVideo()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();

            LOGGER.Info("video Download link called by " + email);
            return View();
        }



        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();
            LOGGER.Info("video upload link called by " + email + " with the filename " + file.FileName);

            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    LOGGER.Info("video upload to azure blob container started for " + email+ "with the filename "+file.FileName);
                    string message = "";
                    CloudBlobContainer _azureBlob = AzureBlob.AzureBlobContainer(VideoContainerName);
                    CloudBlockBlob blockBlob = _azureBlob.GetBlockBlobReference(file.FileName);
                    using (var fileStream = file.InputStream)
                    {
                        try
                        {
                            blockBlob.UploadFromStream(fileStream, accessCondition: Microsoft.WindowsAzure.Storage.AccessCondition.GenerateIfNoneMatchCondition("*"));
                            LOGGER.Info("video upload to azure blob container completed successfully for " + email+ " with the filename "+file.FileName);
                            message = "File Uploaded Successfully";
                        }
                        catch (StorageException ex)
                        {
                            if (ex.RequestInformation.HttpStatusCode == (int)System.Net.HttpStatusCode.Conflict)
                            {
                                LOGGER.Info("The video is already exisit in the " + email + " with the filename " + file.FileName);
                                message = "File Already Exisit";
                            }
                        }

                    }
                    ViewBag.Message = message;
                }
                catch (Exception ex)
                {
                    LOGGER.Error("video uplod has failed for " + email + " with the filename " + file.FileName+ "due to " + ex.Message.ToString());
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("UploadVideo");
        }

        public ActionResult Suggest(bool highlights, bool fuzzy, string term)
        {
            InitSearch();

            // Call suggest API and return results
            SuggestParameters sp = new SuggestParameters()
            {
                UseFuzzyMatching = fuzzy,
                Top = 5
            };

            if (highlights)
            {
                sp.HighlightPreTag = "<b>";
                sp.HighlightPostTag = "</b>";
            }

            var suggestResult = _indexClient.Documents.Suggest(term, "suggestor", sp);

            // Convert the suggest query results to a list that can be displayed in the client.
            List<string> suggestions = suggestResult.Results.Select(x => x.Text).ToList();
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = suggestions
            };
        }

        public ActionResult AutoComplete(string term)
        {
            InitSearch();
            //Call autocomplete API and return results
            AutocompleteParameters ap = new AutocompleteParameters()
            {
                AutocompleteMode = AutocompleteMode.OneTermWithContext,
                UseFuzzyMatching = false,
                Top = 5
            };
            AutocompleteResult autocompleteResult = _indexClient.Documents.Autocomplete(term, "suggestor", ap);

            // Conver the Suggest results to a list that can be displayed in the client.
            List<string> autocomplete = autocompleteResult.Results.Select(x => x.Text).ToList();
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = autocomplete
            };
        }


    }


}