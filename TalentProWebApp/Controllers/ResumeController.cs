using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using NLog;
using TalentProWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Search.Models;
using Microsoft.Azure.Search;

namespace TalentProWebApp.Controllers
{
    [Authorize]
    public class ResumeController : Controller
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

        private readonly int pageSize =Convert.ToInt16(ConfigurationManager.AppSettings["PageSize"]);
        private readonly string DocContainerName = ConfigurationManager.AppSettings["DocContainerName"].ToString();
        private static SearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        private static string IndexName = ConfigurationManager.AppSettings["DocIndexName"];

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

            LOGGER.Info("Index Page called by "+email);


            return View();
        }


        public ActionResult UploadDocument()
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();

            LOGGER.Info("Document Download link called by " + email);
            return View();
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();
            LOGGER.Info("Document upload link called by " + email + " with the filename " + file.FileName);
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    LOGGER.Info("Document upload to azure blob container started for " + email + " with the filename " + file.FileName);

                    CloudBlobContainer _azureBlob = AzureBlob.AzureBlobContainer(DocContainerName);
                    CloudBlockBlob blockBlob = _azureBlob.GetBlockBlobReference(file.FileName);
                    using (var fileStream = file.InputStream)
                    {
                        blockBlob.UploadFromStream(fileStream);
                    }
                    LOGGER.Info("Document upload to azure blob container completed successfully for " + email + " with the filename " + file.FileName);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    LOGGER.Error("Document uplod has failed for " + email + " with the filename " + file.FileName+ "due to " +ex.Message.ToString());
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("UploadDocument");
        }

    
        [ActionName("Download")]
        [HttpPost]
        public FileResult Download(string fileURL)
        {
            var webClient = new WebClient();
            byte[] fileBytes = webClient.DownloadData(fileURL);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, System.IO.Path.GetFileName(HttpUtility.UrlDecode(fileURL).Trim()));
        }


        [ActionName("Search")]
        public async Task<ActionResult> Search(string search, int pageNo=0,DocumentType documentType = DocumentType.Resume)
        {
            ClaimsPrincipal cp = ClaimsPrincipal.Current;
            string email = cp.Identity.Name.ToString();
            LOGGER.Info("Search Document link called by " + email +"parameters are "+search+"page number "+pageNo );


            AzSearchModel azSearchModel = new AzSearchModel { search = search, skip = pageNo* pageSize, top = pageSize, highlightPostTag="</b>",highlightPreTag= "<b>", highlight= "keys" };

            var items = await GetAzureSearchResults.GetDocListAsync<DocumentModel>(azSearchModel);

            ViewData["pageNo"] = pageNo;
            ViewData["pageSize"] = pageSize;
            ViewData["totalRemainingPageSize"] = (int)Math.Ceiling((double)items.odatacount / pageSize)- pageNo;

            LOGGER.Info("Search Document link called by " + email + "parameters are " + search + "page number " + pageNo +"completed sucessfully");

            return View(items.value);

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

            var suggestResult = _indexClient.Documents.Suggest(term, "suggesters", sp);

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
            AutocompleteResult autocompleteResult = _indexClient.Documents.Autocomplete(term, "suggesters", ap);

            // Conver the Suggest results to a list that can be displayed in the client.
            List<string> autocomplete = autocompleteResult.Results.Select(x => x.Text).ToList();
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = autocomplete
            };
        }

        public static string FindImagewithPersonTag (List<CognitiveImageAnalysis> cognitiveImageAnalyses)
        {
            string url = cognitiveImageAnalyses.Where(record=>record.imageAnalysis.Description!=null).Where(a => a.imageAnalysis.Description.Tags.Any(tags => tags.IndexOf("person", StringComparison.OrdinalIgnoreCase) >= 0)).FirstOrDefault()?.imageURL;
            return url;
        }


        public static string FindDetails(List<EntityRecord> entityRecords, ExtractType extractType)
        {
            string value = "";

            switch (extractType)
            {
                case ExtractType.Name:
                    string name = entityRecords.Where(a => a.Type == "Person").FirstOrDefault()?.Name;
                    if (name != null)
                    {
                        value = name.Substring(0, name.Length > 20 ? 20 : name.Length);
                    }
                    break;

                case ExtractType.Email:
                    value = entityRecords.Where(a => a.Type == "Email" && a.WikipediaUrl == null).FirstOrDefault()?.Name;
                    break;

                case ExtractType.Phone:
                    List<string> numberList = entityRecords.Where(a => a.Type == "Quantity" && a.SubType == "Number").Select(a => a.Name).ToList();
                    value = numberList.Where(a => a.Length >= 10).FirstOrDefault();
                    break;

                case ExtractType.LinkedIn:
                    List<string> urlList = entityRecords.Where(a => a.Type == "URL").Select(a => a.Name).ToList();
                    value = urlList.Where(a => a.Contains("linkedin")).FirstOrDefault();
                    break;


            }



            return value;
        }
    }
}