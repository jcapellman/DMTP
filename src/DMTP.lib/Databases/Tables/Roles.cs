using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class Roles : BaseTable
    {
        public string Name { get; set; }

        public bool Active { get; set; }

        public bool BuiltIn { get; set; }

        public bool UsersEdit { get; set; }

        public bool UsersView { get; set; }

        public bool UsersDelete { get; set; }

        public bool SettingsEdit { get; set; }

        public bool SettingsView { get; set; }

        public bool CanUploadModels { get; set; }

        public bool CanDownloadModels { get; set; }

        public bool RolesDelete { get; set; }

        public bool RolesEdit { get; set; }

        public bool RolesView { get; set; }
    }
}