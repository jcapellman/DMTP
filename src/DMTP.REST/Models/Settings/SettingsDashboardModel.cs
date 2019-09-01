namespace DMTP.REST.Models.Settings
{
    public class SettingsDashboardModel
    {
        public lib.dal.Databases.Tables.Settings Setting { get; set; }

        public string ActionMessage { get; set; }

        public bool GenerateNewRegistrationKey { get; set; }
    }
}