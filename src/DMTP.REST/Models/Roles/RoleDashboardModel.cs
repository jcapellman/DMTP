using System.Collections.Generic;

using DMTP.REST.Auth;

namespace DMTP.REST.Models.Roles
{
    public class RoleDashboardModel
    {
        public RoleDashboardModel(ApplicationUser user)
        {
            CurrentUser = user;
        }

        public ApplicationUser CurrentUser { get; set; }

        public List<RoleListingItem> RolesListing { get; set; }

        public string ActionMessage { get; set; }
    }
}