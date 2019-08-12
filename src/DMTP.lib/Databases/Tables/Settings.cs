using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class Settings : BaseTable
    {
        public string SMTPHostName { get; set; }

        public int SMTPPortNumber { get; set; }

        public string SMTPUsername { get; set; }

        public string SMTPPassword { get; set; }

        public bool AllowNewUserCreation { get; set; }
    }
}