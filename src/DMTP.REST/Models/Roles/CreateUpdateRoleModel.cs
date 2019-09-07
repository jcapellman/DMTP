using System;
using System.Collections.Generic;

using DMTP.lib.dal.Enums;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Models.Roles
{
    public class CreateUpdateRoleModel
    {
        public Guid? ID { get; set; }

        public string ActionMessage { get; set; }

        public string Name { get; set; }

        public Dictionary<AccessSections, AccessLevels> Permissions { get; set; }

        public List<SelectListItem> AccessLevels { get; set; }
    }
}