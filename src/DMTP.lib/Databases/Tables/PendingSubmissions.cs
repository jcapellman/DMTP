using System;

namespace DMTP.lib.Databases.Tables
{
    public class PendingSubmissions
    {
        public Guid ID { get; set; }

        public string JobJSON { get; set; }
    }
}