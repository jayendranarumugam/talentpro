using System.Collections.Generic;
using System.Web;

namespace TalentProWebApp.Models
{
    public class UploadFileModel
    {
        public UploadFileModel()
        {
           
        }

        public HttpPostedFileBase File { get; set; }
        public string FileType { get; set; }
        // Rest of model details
    }
}