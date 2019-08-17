using System.Collections.Generic;

using DMTP.REST.Auth;

namespace DMTP.REST.Models.Roles
{
    public class RoleDashboardModel : BaseModel
    {
        public RoleDashboardModel(ApplicationUser user) : base(user)
        {
        }

        public List<RoleListingItem> RolesListing { get; set; }

        public string ActionMessage { get; set; }
    }
}