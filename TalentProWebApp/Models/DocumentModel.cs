using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Newtonsoft.Json;

namespace TalentProWebApp.Models
{
    public class DocumentModel : IAzResutValueModel
    {
        public string id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUri { get; set; }
        public List<string> imageDetails { get; set; }
        public List<string> keys { get; set; }

        [JsonIgnore]
        public string ProfilePic { get; set; }

        //[JsonIgnore]
        public List<EntityRecord> EntityRecordslist { get; set; }

        //[JsonIgnore]
        public List<CognitiveImageAnalysis> imageAnalyses { get; set; }

        //[JsonIgnore]
        //public List<CognitiveImageTextAnalysis> imageTextAnalyses { get; set; }
    }
   
}