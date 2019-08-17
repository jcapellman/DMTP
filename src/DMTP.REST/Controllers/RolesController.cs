using System.Linq;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.REST.Models.Roles;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        protected RolesController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index(string actionMessage = null)
        {
            var model = new RoleDashboardModel();

            var users = Database.GetUsers();

            model.RolesListing = Database.GetRoles().Select(a => new RoleListingItem
            {
                Name = a.Name,
                ID = a.ID,
                NumberAssociatedUsers = users.Count(b => b.RoleID == a.ID)
            }).ToList();

            if (!string.IsNullOrEmpty(actionMessage))
            {
                model.ActionMessage = actionMessage;
            }

            return View(model);
        }
    }
}