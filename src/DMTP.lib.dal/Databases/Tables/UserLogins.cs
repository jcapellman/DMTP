using System;

using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class UserLogins : BaseTable
    {
        public Guid? UserID { get; set; }
        
        public DateTimeOffset Timestamp { get; set; }

        public string Username { get; set; }

        public bool Successful { get; set; }

        public string IPAddress { get; set; }
    }
}