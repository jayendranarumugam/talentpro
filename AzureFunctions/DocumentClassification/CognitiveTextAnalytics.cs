using iTextSharp.text.pdf;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DocumentClassification
{
    class CognitiveTextAnalytics
    {
        private static string TextAnalyticsSubscriptionKey = Environment.GetEnvironmentVariable("TextAnalyticsSubscriptionKey");
        private static string TextAnalyticsEndPoint = Environment.GetEnvironmentVariable("TextAnalyticsEndPoint");
        private static int MaxLengthofCharacters = Convert.ToInt32(Environment.GetEnvironmentVariable("TextAnalyticsMaxLenghChar"));
        class ApiKeyServiceClientCredentialsForText : ServiceClientCredentials
        {
            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", TextAnalyticsSubscriptionKey);
                return base.ProcessHttpRequestAsync(request, cancellationToken);
            }
        }
        public static void TextAnalytics(PdfReader pdfReader, TraceWriter log, ResumeDocModel resumeDocModel)
        {
            // Create a client.
            ITextAnalyticsClient client = new TextAnalyticsClient(new ApiKeyServiceClientCredentialsForText())
            {
                Endpoint = TextAnalyticsEndPoint
            }; //Replace 'westus' with the correct region for your Text Analytics subscription

            Console.OutputEncoding = System.Text.Encoding.UTF8;



            // Extracting language
            log.Info("===== Text Analytics Started ======");
            string content = DocumentExtraction.GetTextFromPDF(pdfReader);


            List<string> splittedList = StringExtensions.Split(content, MaxLengthofCharacters).ToList();

            var LanguageDetectAPI = client.DetectLanguageAsync(new BatchInput(
                        new List<Input>()
                            {
                        new Input(resumeDocModel.DocumentName,splittedList.First())
                        })).Result;

            resumeDocModel.languageBatchResult = LanguageDetectAPI.Documents.FirstOrDefault();

            var detectedLanguage = LanguageDetectAPI.Documents.Select(doc => doc.DetectedLanguages[0].Iso6391Name).FirstOrDefault();

            //SentimentBatchResult result3 = client.SentimentAsync(
            //   new MultiLanguageBatchInput(
            //       new List<MultiLanguageInput>()
            //       {
            //              new MultiLanguageInput(detectedLanguage, resumeDocModel.DocumentName, splittedList.First())
            //       })).Result;

            //resumeDocModel.sentimentBatchResult = result3.Documents.FirstOrDefault();

            List<string> keyPhraseList = new List<string>();

            List<EntityRecordV2dot1> entityRecords = new List<EntityRecordV2dot1>();

            foreach (string splittedContent in splittedList)
            {

                KeyPhraseBatchResult keyPhraseBatch = client.KeyPhrasesAsync(new MultiLanguageBatchInput(
                            new List<MultiLanguageInput>()
                            {
                          new MultiLanguageInput(detectedLanguage, resumeDocModel.DocumentName, splittedContent)
                            })).Result;

                foreach (var doc in keyPhraseBatch.Documents)
                {
                    keyPhraseList.AddRange(doc.KeyPhrases.ToList());

                }


                EntitiesBatchResultV2dot1 entitiesbatchres = client.EntitiesAsync(
                 new MultiLanguageBatchInput(
                     new List<MultiLanguageInput>()
                     {
                          new MultiLanguageInput(detectedLanguage, resumeDocModel.DocumentName, splittedContent)
                     })).Result;

                entityRecords.AddRange(entitiesbatchres.Documents.First().Entities.ToList());                    

            }

            resumeDocModel.keyPhraseBatchResult.Id = resumeDocModel.DocumentName;
            resumeDocModel.keyPhraseBatchResult.KeyPhrases = keyPhraseList;

            resumeDocModel.entityBatchResult.Id = resumeDocModel.DocumentName;
            resumeDocModel.entityBatchResult.EntityRecords = entityRecords;



            log.Info("===== Text Analytics Completed ======");

        }

    }
}
