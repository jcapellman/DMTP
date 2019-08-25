using System.Collections.Generic;

using DMTP.lib.dal.Databases.Tables.Base;
using DMTP.lib.dal.Enums;
using DMTP.lib.Enums;

namespace DMTP.lib.dal.Databases.Tables
{
    public class Roles : BaseTable
    {
        public string Name { get; set; }

        public bool BuiltIn { get; set; }

        public Dictionary<AccessSections, AccessLevels> Permissions { get; set; }
    }
}