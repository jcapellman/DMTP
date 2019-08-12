using System.Collections.Generic;

using DMTP.lib.Databases.Tables;

namespace DMTP.REST.Models.Users
{
    public class UserDashboardModel
    {
        public List<UserListingItem> UsersListing { get; set; }

        public List<UserLogins> UserLoginListing { get; set; }

        public string ActionMessage { get; set; }
    }
}