using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class PendingSubmissions : BaseTable
    {
        public string JobJSON { get; set; }
    }
}