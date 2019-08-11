using System.Collections.Generic;

using DMTP.lib.Databases.Tables;

namespace DMTP.REST.Models
{
    public class UserListingModel
    {
        public List<Users> UsersListing { get; set; }

        public List<UserLogins> UserLoginListing { get; set; }

        public string ActionMessage { get; set; }
    }
}