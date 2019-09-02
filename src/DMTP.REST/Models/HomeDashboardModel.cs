using System.Collections.Generic;

using DMTP.lib.dal.Databases.Tables;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Models
{
    public class HomeDashboardModel
    {
        public List<lib.dal.Databases.Tables.Workers> Workers { get; set; }
        
        public List<Jobs> Jobs { get; set; }

        public List<SelectListItem> ModelTypes { get; set; }

        public List<SelectListItem> AssignWorkers { get; set; }
    }
}