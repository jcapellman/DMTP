using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class Settings : BaseTable
    {
        public bool IsInitialized { get; set; }

        public string SMTPHostName { get; set; }

        public int SMTPPortNumber { get; set; }

        public string SMTPUsername { get; set; }

        public string SMTPPassword { get; set; }

        public bool AllowNewUserCreation { get; set; }

        public string DeviceKeyPassword { get; set; }
    }
}