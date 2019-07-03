using DMTP.lib.Databases.Tables.Base;

namespace DMTP.lib.Databases.Tables
{
    public class PendingSubmissions : BaseTable
    {
        public string JobJSON { get; set; }
    }
}