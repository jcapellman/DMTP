using System;
using System.Linq;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.REST.Models.Roles;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        protected override AccessSections CurrentSection => AccessSections.ROLES;

        public RolesController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        [HttpGet]
        public IActionResult DeleteRole(Guid id)
        {
            if (!HasAccess(AccessLevels.FULL))
            {
                return RedirectNotAuthorized();
            }

            var result = Database.DeleteRole(id);

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully deleted user" : "Failed to delete user" });
        }

        public IActionResult Index(string actionMessage = null)
        {
            if (!HasAccess(AccessLevels.VIEW_ONLY))
            {
                return RedirectNotAuthorized();
            }

            var model = new RoleDashboardModel(GetApplicationUser());

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