using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DocumentClassification
{
    class CognitiveComputerVision
    {
        private static string VisionSubscriptionKey = Environment.GetEnvironmentVariable("VisionSubscriptionKey");
        private static string ComputerVisionEndpoint = Environment.GetEnvironmentVariable("ComputerVisionEndpoint");
        private const TextRecognitionMode textRecognitionMode = TextRecognitionMode.Printed;
        private const int numberOfCharsInOperationId = 36;
        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Description,VisualFeatureTypes.Faces,
            VisualFeatureTypes.Tags
        };

        public static async Task VisionAnalyticsAsync(TraceWriter log, string fileURLWithSAS, ImageBatchResult imageBatchResult)
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(VisionSubscriptionKey),
                   new DelegatingHandler[] { });


            computerVision.Endpoint = ComputerVisionEndpoint;
            string imageURLwithoutSAS = fileURLWithSAS.Substring(0, fileURLWithSAS.IndexOf('?'));

            CognitiveImageAnalysis cogniiveImageAnalysis = new CognitiveImageAnalysis { imageURL = imageURLwithoutSAS };
            CognitiveImageTextAnalysis cognitiveImageTextAnalysis = new CognitiveImageTextAnalysis { imageURL = imageURLwithoutSAS };

            var t1 = AnalyzeRemoteAsync(computerVision, fileURLWithSAS, cogniiveImageAnalysis);
            var t2 = ExtractRemoteTextAsync(computerVision, fileURLWithSAS, log, cognitiveImageTextAnalysis);
            await Task.WhenAll(t1, t2);

            imageBatchResult.imageAnalyses.Add(cogniiveImageAnalysis);
            imageBatchResult.imageTextAnalyses.Add(cognitiveImageTextAnalysis);
        }

        private static async Task AnalyzeRemoteAsync(ComputerVisionClient computerVision, string fileURLWithSAS, CognitiveImageAnalysis cognitiveImageAnalysis)
        {
            try
            {
                if (ValidateImage(1, fileURLWithSAS))
                {
                    cognitiveImageAnalysis.imageAnalysis = await computerVision.AnalyzeImageAsync(fileURLWithSAS, features);
                }
                else
                {
                    cognitiveImageAnalysis.error = "Image Validation Failed for ImageAnalysis API";

                }

            }
            catch (Exception e)
            {
                if (e is ComputerVisionErrorException)
                {
                    ComputerVisionErrorException ex = (ComputerVisionErrorException)e;
                    cognitiveImageAnalysis.error = ex.Message.ToString() + " due to " + ex.Response.Content;
                }
                else
                {
                    cognitiveImageAnalysis.error = e.Message.ToString();
                }

            }

        }

        private static bool ValidateImage(int apiType, string fileURL)
        {
            bool validimage = false;
            if (fileURL != null)
            {
                byte[] imageData = new WebClient().DownloadData(fileURL);
                MemoryStream imgStream = new MemoryStream(imageData);
                Image img = Image.FromStream(imgStream);

                int wSize = img.Width;
                int hSize = img.Height;

                if (apiType == 1) //ImageAnalysis
                {
                    if (wSize > 50 && hSize > 50)
                    {
                        validimage = true;
                    }
                }
                else if (apiType == 2)//ExtractTextFromImage
                {
                    if ((wSize > 50 && wSize < 4200) && (hSize > 50 && hSize < 4200))
                    {
                        validimage = true;
                    }
                }
            }
            return validimage;
        }

        
        private static async Task ExtractRemoteTextAsync(ComputerVisionClient computerVision, string fileURLWithSAS, TraceWriter log, CognitiveImageTextAnalysis cognitiveImageTextAnalysis)
        {
            try
            {
                if (ValidateImage(2, fileURLWithSAS))
                {
                    RecognizeTextHeaders textHeaders = await computerVision.RecognizeTextAsync(fileURLWithSAS, textRecognitionMode);
                    await GetTextAsync(computerVision, textHeaders.OperationLocation, log, cognitiveImageTextAnalysis);
                }
                else
                {
                    cognitiveImageTextAnalysis.error = "Image Validation Failed for ImageTextAnalytics API ";

                }

                
            }
            catch (Exception e)
            {
                if (e is ComputerVisionErrorException)
                {
                    ComputerVisionErrorException ex = (ComputerVisionErrorException)e;
                    cognitiveImageTextAnalysis.error = ex.Message.ToString() + " due to " + ex.Response.Content;
                }
                else
                {
                    cognitiveImageTextAnalysis.error = e.Message.ToString();
                }

            }
        }
        private static async Task GetTextAsync(ComputerVisionClient computerVision, string operationLocation, TraceWriter log, CognitiveImageTextAnalysis cognitiveImageTextAnalysis)
        {
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            log.Info("\nCalling GetHandwritingRecognitionOperationResultAsync()");
            TextOperationResult result = await computerVision.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                log.Info("Server status: " + result.Status + ", waiting " + i + " seconds...");
                await Task.Delay(1000);

                result = await computerVision.GetTextOperationResultAsync(operationId);
            }

            cognitiveImageTextAnalysis.textOperationResult = result;

        }
    }
}
