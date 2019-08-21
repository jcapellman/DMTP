using System;

using DMTP.lib.Databases.Tables;

namespace DMTP.lib.Auth
{
    public class ApplicationUser
    {
        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Roles Role { get; set; }
    }
}