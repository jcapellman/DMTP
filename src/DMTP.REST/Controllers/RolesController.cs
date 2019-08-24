using System;
using System.Linq;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.REST.Attributes;
using DMTP.REST.Models.Roles;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        public RolesController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        [HttpGet]
        [Access(AccessSections.ROLES, AccessLevels.EDIT)]
        public IActionResult Edit(Guid id)
        {
            var roles = Database.GetRoles();

            var currentRole = roles.FirstOrDefault(a => a.ID == id);

            if (currentRole == null || currentRole.BuiltIn)
            {
                return RedirectToAction("Index", new { actionMessage = "Cannot modify a built-in role"});
            }

            var model = new CreateUpdateRoleModel
            {
                ID = id,
                Name = currentRole.Name,
                Permissions = currentRole.Permissions,
                ActionMessage = string.Empty,
                AccessLevels = Enum.GetNames(typeof(AccessLevels)).Select(a => new SelectListItem(a, a)).ToList()
            };

            return View(model);
        }

        [HttpGet]
        [Access(AccessSections.ROLES, AccessLevels.EDIT)]
        public IActionResult Create()
        {
            var permissions = Enum.GetNames(typeof(AccessSections)).ToDictionary(section => Enum.Parse<AccessSections>(section), section => AccessLevels.EDIT);

            var model = new CreateUpdateRoleModel
            {
                Name = string.Empty,
                Permissions = permissions,
                ActionMessage = string.Empty,
                AccessLevels = Enum.GetNames(typeof(AccessLevels)).Select(a => new SelectListItem(a, a)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult AttemptUpdate(CreateUpdateRoleModel model)
        {
            var result = Database.UpdateRole(model.ID.Value, model.Name, model.Permissions);
            
            return RedirectToAction("Index", new { actionMessage = result ? "Successfully edited role" : "Failed to edit role" });
        }

        [HttpPost]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult AttemptCreate(CreateUpdateRoleModel model)
        {
            var result = Database.CreateRole(model.Name, false, model.Permissions);
        
            return RedirectToAction("Index", new { actionMessage = result.HasValue ? "Successfully created role" : "Failed to create role" });
        }

        [HttpGet]
        [Access(AccessSections.ROLES, AccessLevels.FULL)]
        public IActionResult DeleteRole(Guid id)
        {
            var result = Database.DeleteRole(id);

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully deleted role" : "Failed to delete role" });
        }

        [Access(AccessSections.ROLES, AccessLevels.VIEW_ONLY)]
        public IActionResult Index(string actionMessage = null)
        {
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