using System.Collections.Generic;
using DMTP.lib.Databases.Tables;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Models
{
    public class HomeDashboardModel
    {
        public List<Hosts> Hosts { get; set; }
        
        public List<Jobs> Jobs { get; set; }

        public List<SelectListItem> ModelTypes { get; set; }
    }
}