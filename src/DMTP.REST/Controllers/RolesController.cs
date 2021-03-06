﻿using System;
using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;
using DMTP.REST.Attributes;
using DMTP.REST.Models.Roles;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly IStringLocalizer<RolesController> _localizer;

        private readonly RoleManager _roleManager;

        public RolesController(DatabaseManager database, Settings settings, IStringLocalizer<RolesController> localizer) : base(database, settings)
        {
            _roleManager = new RoleManager(Database);

            _localizer = localizer;
        }

        [HttpGet]
        [Access(AccessSections.ROLES, AccessLevels.EDIT)]
        public IActionResult Edit(Guid id)
        {
            var roles = _roleManager.GetRoles();

            var currentRole = roles.FirstOrDefault(a => a.ID == id);

            if (currentRole == null || currentRole.BuiltIn)
            {
                return RedirectToAction("Index", new { actionMessage = _localizer["CantModifyBuiltInRole"] });
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
            var permissions = Enum.GetNames(typeof(AccessSections)).ToDictionary(Enum.Parse<AccessSections>, section => AccessLevels.EDIT);

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
            var result = _roleManager.UpdateRole(model.ID.Value, model.Name, model.Permissions);
            
            return RedirectToAction("Index", new
            {
                actionMessage = result ? _localizer["SuccessfullyEditedRole"] : _localizer["FailedToEditRole"]
            });
        }

        [HttpPost]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult AttemptCreate(CreateUpdateRoleModel model)
        {
            var result = _roleManager.CreateRole(model.Name, false, model.Permissions);
        
            return RedirectToAction("Index", new
            {
                actionMessage = result.HasValue ? 
                    _localizer["SuccessfullyCreatedRole"] : _localizer["FailedToCreateRole"]
            });
        }

        [HttpGet]
        [Access(AccessSections.ROLES, AccessLevels.FULL)]
        public IActionResult DeleteRole(Guid id)
        {
            var result = _roleManager.DeleteRole(id);

            return RedirectToAction("Index", new { actionMessage = result ? 
                _localizer["SuccessfullyDeletedRole"] : _localizer["FailedToDeleteRole"] });
        }

        [Access(AccessSections.ROLES, AccessLevels.VIEW_ONLY)]
        public IActionResult Index(string actionMessage = null)
        {
            var model = new RoleDashboardModel(GetApplicationUser());

            var users = new UserManager(Database).GetUsers();

            model.RolesListing = _roleManager.GetRoles().Select(a => new RoleListingItem
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