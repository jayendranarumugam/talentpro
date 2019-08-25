using System;
using System.Collections.Generic;

namespace TalentProWebApp.Models
{
    public class VideoModel : IAzResutValueModel
    {
        public string id { get; set; }

        public string accountId { get; set; }

        public List<videoIndexerModel> video { get; set; }
    }

    public class videoIndexerModel
    {
        public string token { get; set; }
    }
}