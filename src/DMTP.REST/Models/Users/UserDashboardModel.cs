using System.Collections.Generic;

using DMTP.lib.Auth;
using DMTP.lib.dal.Databases.Tables;

namespace DMTP.REST.Models.Users
{
    public class UserDashboardModel : BaseModel
    {
        public List<UserListingItem> UsersListing { get; set; }

        public List<UserLogins> UserLoginListing { get; set; }

        public string ActionMessage { get; set; }

        public UserDashboardModel(ApplicationUser user) : base(user)
        {
        }
    }
}