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

namespace TalentProWebApp.Controllers
{
    [Authorize]
    public class VideoController : Controller
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();

        private readonly int pageSize = Convert.ToInt16(ConfigurationManager.AppSettings["PageSizeForVideo"]);
        private readonly string VideoContainerName = ConfigurationManager.AppSettings["VideoContainerName"].ToString();


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
                            blockBlob.UploadFromStream(fileStream, accessCondition: AccessCondition.GenerateIfNoneMatchCondition("*"));
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


    }


}