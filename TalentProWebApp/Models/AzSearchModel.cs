using Newtonsoft.Json;
using System.Collections.Generic;

namespace TalentProWebApp.Models
{
    public class AzSearchModel
    {
        public string search { get; set; }
        public string filter { get; set; }
        public string facets { get; set; }
        public string highlightPreTag { get; set; }
        public string highlightPostTag { get; set; }
        public bool count { get; set; } = true;
        public int skip { get; set; }
        public int top { get; set; }

    }

    public interface IAzResutValueModel { }



    public class AzSearchResultModel<T> : AzSearchResultOdataModel where T : IAzResutValueModel
    {       
        public List<T> value { get; set; }

    }


    public class AzSearchResultOdataModel
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string odatacontext { get; set; }

        [JsonProperty(PropertyName = "@OData.count")]
        public int odatacount { get; set; }
    }
}