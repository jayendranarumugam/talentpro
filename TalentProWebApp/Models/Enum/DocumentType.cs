using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TalentProWebApp.Models
{
    public enum DocumentType
    {
        Resume=1,
        IAD=2
    }

    public enum VideoType
    {
        Resume=1,
        Others=2
    }

    public enum ExtractType
    {
        Name,
        Email,
        LinkedIn,
        Phone
    }
}