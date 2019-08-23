using System;
using System.Collections.Generic;

using DMTP.lib.Enums;

namespace DMTP.REST.Models.Roles
{
    public class CreateUpdateRoleModel
    {
        public Guid? ID { get; set; }

        public string ActionMessage { get; set; }

        public string Name { get; set; }

        public Dictionary<AccessSections, AccessLevels> Permissions { get; set; }
    }
}