using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class Roles : BaseTable
    {
        public string Name { get; set; }

        public bool BuiltIn { get; set; }

        public bool CanEditUsers { get; set; }

        public bool CanEditSettings { get; set; }
    }
}