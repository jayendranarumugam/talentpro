using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DocumentClassification
{
   public class ResumeDocModel
    {
        public ResumeDocModel()
        {
            this.keyPhraseBatchResult = new KeyPhraseResultItem();
            this.imageDetails = new ImageDetails();
            this.imageBatchResult = new ImageBatchResult();
            this.entityBatchResult = new EntityBatchResultItem();
        }

        public DocType docType { get; set; }
        private string _id;
        [JsonProperty(PropertyName = "id")]
        public string id
        {
            get
            {
                if (_id == null)
                {
                    _id = Guid.NewGuid().ToString();
                }
                return _id;
            }

            set { _id = value; }
        } 
        public string DocumentName { get; set; }
        public string DocumentUri { get; set; }
        public LanguageBatchResultItem languageBatchResult { get; set; }
        public KeyPhraseResultItem keyPhraseBatchResult { get; set; }
        public SentimentBatchResultItem sentimentBatchResult { get; set; }
        public EntityBatchResultItem entityBatchResult { get; set; }
        public ImageDetails imageDetails { get; set; }
        public ImageBatchResult imageBatchResult{ get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class KeyPhraseResultItem
    {
        public KeyPhraseResultItem()
        {
            KeyPhrases = new List<string>();
            Id = null;
        }
        public List<string> KeyPhrases { get; set; }
        public string Id { get; set; }
    }

    public class EntityBatchResultItem
    {
        public EntityBatchResultItem()
        {
            EntityRecords = new List<EntityRecordV2dot1>();
            Id = null;
        }
        public List<EntityRecordV2dot1> EntityRecords { get; set; }
        public string Id { get; set; }
    }

    public class ImageDetails
    {
        public ImageDetails()
        {
            this.imageURLList = new List<string>();
        }
        public List<string> imageURLList { get; set; }
    }
    public class ImageBatchResult
    {
        public ImageBatchResult()
        {
            imageAnalyses = new List<CognitiveImageAnalysis>();
            imageTextAnalyses=new List<CognitiveImageTextAnalysis>();
        }
        public List<CognitiveImageAnalysis> imageAnalyses { get; set; }
        public List<CognitiveImageTextAnalysis> imageTextAnalyses { get; set; }
    
    }
    public class CognitiveImageAnalysis
    {
        public CognitiveImageAnalysis()
        {
            this.imageURL = null;
            this.imageAnalysis = new ImageAnalysis();
        }
        public string imageURL { get; set; }
        public string error { get; set; }

        public ImageAnalysis imageAnalysis { get; set; }
    }

    public class CognitiveImageTextAnalysis
    {
        public CognitiveImageTextAnalysis()
        {
            this.imageURL = null;
            this.textOperationResult = new TextOperationResult();
        }
        public string imageURL { get; set; }
        public string error { get; set; }
        public TextOperationResult textOperationResult { get; set; }
    }

    public enum DocType
    {
        Resume=1,
        Others=2
    }
}
