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


        public List<EntityRecord> EntityRecordslist { get; set; }

        public List<CognitiveImageAnalysis> imageAnalyses { get; set; }

        [JsonProperty(PropertyName = "@search.highlights")]
        public SearchHighLights searchHighLights { get; set; }
    }

    public class SearchHighLights
    {
        public List<string> keys { get; set; }
    }


}