using System.Collections.Generic;

namespace DMTP.REST.Models.Roles
{
    public class RoleDashboardModel
    {
        public List<RoleListingItem> RolesListing { get; set; }

        public string ActionMessage { get; set; }
    }
}