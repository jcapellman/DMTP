using System.Collections.Generic;

using DMTP.lib.Databases.Tables.Base;
using DMTP.lib.Enums;

namespace DMTP.lib.Databases.Tables
{
    public class Roles : BaseTable
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public bool BuiltIn { get; set; }

        public Dictionary<AccessSections, AccessLevels> Permissions { get; set; }
    }
}