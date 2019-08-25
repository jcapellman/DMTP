using System;

using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class Workers : BaseTable
    {
        public string Name { get; set; }

        public int NumCores { get; set; }

        public string OSVersion { get; set; }

        public DateTime LastConnected { get; set; }
        
        public string WorkerVersion { get; set; }
    }
}