using System;

using DMTP.lib.dal.Databases.Tables.Base;

namespace DMTP.lib.dal.Databases.Tables
{
    public class Users : BaseTable
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public Guid RoleID { get; set; }
    }
}