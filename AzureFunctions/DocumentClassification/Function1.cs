using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DocumentClassification
{
    public static class Function1
    {
        private static string[] _validExtensions = { "pdf" };
        private static readonly string ResumeKeyWords = Environment.GetEnvironmentVariable("ResumeKeyWords");

        [FunctionName("Function1")]
        public static async Task RunAsync([BlobTrigger("resumeuploadcontainer/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, Uri uri, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            string[] spiltfilenamewithext = name.Split('.');
            string filenamewithoutextension = spiltfilenamewithext[0];

            if (IsValidExtension(spiltfilenamewithext[spiltfilenamewithext.Length - 1]))
            {
                PdfReader pdfReader = new PdfReader(myBlob);
                ResumeDocModel resumeDocModel = new ResumeDocModel { DocumentName = filenamewithoutextension, DocumentUri = uri.AbsoluteUri };
                CognitiveTextAnalytics.TextAnalytics(pdfReader, log, resumeDocModel);
                string[] resumeKeysArray = ResumeKeyWords.Split(new string[] { "," }, StringSplitOptions.None);

                bool isResumeDoc = false;
                foreach (string keys in resumeKeysArray)
                {
                    isResumeDoc = resumeDocModel.keyPhraseBatchResult.KeyPhrases.Any(s => s.IndexOf(keys, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (isResumeDoc)
                    {
                        resumeDocModel.docType = DocType.Resume;
                        break;
                    }
                }
                if (!isResumeDoc)
                {
                    resumeDocModel.docType = DocType.Others;
                }


                List<CloudBlockBlob> cloudBlocks = DocumentExtraction.ExtractImageUploadToAzure(pdfReader, myBlob, log, filenamewithoutextension, resumeDocModel);

                if (cloudBlocks.Count > 0)
                {
                    log.Info("===== Computer Vision Analysis Started ======");
                    ImageBatchResult imageBatchResult = new ImageBatchResult();
                    foreach (CloudBlockBlob cloudBlockBlob in cloudBlocks)
                    {
                        string blobUrlWithSAS = AzStorage.GetBlobSasUri(cloudBlockBlob);
                        await CognitiveComputerVision.VisionAnalyticsAsync(log, blobUrlWithSAS, imageBatchResult);
                    }
                    resumeDocModel.imageBatchResult = imageBatchResult;
                    log.Info("===== Computer Vision Analysis Completed ======");
                }
                else
                {
                    log.Info("The Document doesn't not have any Images to analyze");
                }

                CosmosDB cosmosDB = new CosmosDB();
                cosmosDB.CreateDocumentDB().Wait();
                await cosmosDB.UpdInsResumeDocumentAsync(resumeDocModel);


            }

            else
            {
                log.Info("Please upload the valid Document in " + string.Join(",", _validExtensions) + " Extension");
            }



        }

        public static bool IsValidExtension(string ext)
        {
            return _validExtensions.Contains(ext.ToLower());
        }

    }

}
