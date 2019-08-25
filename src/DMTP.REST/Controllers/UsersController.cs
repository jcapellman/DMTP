using System;
using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.dal.Manager;
using DMTP.lib.Enums;
using DMTP.lib.Managers;
using DMTP.REST.Attributes;
using DMTP.REST.Models.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager _userManager;

        public UsersController(DatabaseManager database, Settings settings) : base(database, settings)
        {
            _userManager = new UserManager(Database);
        }

        [Access(AccessSections.USERS, AccessLevels.VIEW_ONLY)]
        public IActionResult Index(string actionMessage = null)
        {
            var model = new UserDashboardModel(GetApplicationUser())
            {
                UserLoginListing = new LoginManager(Database).GetLogins().OrderByDescending(a => a.Timestamp).ToList(),
            };

            var jobs = new JobManager(Database).GetJobs();

            model.UsersListing = _userManager.GetUsers().Select(a => new UserListingItem
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                EmailAddress = a.EmailAddress,
                ID = a.ID,
                LastLogin = model.UserLoginListing.Where(b => b.UserID == a.ID).DefaultIfEmpty().Max(b => b?.Timestamp),
                NumJobs = jobs.Count(b => b.SubmittedByUserID == a.ID)
            }).ToList();
    
           if (!string.IsNullOrEmpty(actionMessage))
           {
               model.ActionMessage = actionMessage;
           }

           return View(model);
        }

        [HttpGet]
        public IActionResult SendEmail(string email)
        {
            SendEmail(email, "Welcome to DMTP", "Welcome");

            return RedirectToAction("Index", "Users", new {actionMessage = $"Email sent to {email}"});
        }

        [HttpGet]
        [Access(AccessSections.USERS, AccessLevels.FULL)]
        public IActionResult DeleteUser(Guid id)
        {
            var result = _userManager.DeleteUser(id);

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully deleted user" : "Failed to delete user"});
        }

        [HttpGet]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult Edit(Guid id)
        {
            var user = _userManager.GetUser(id);

            var roles = new RoleManager(Database).GetRoles();

            var model = new EditUserModel
            {
                FirstName = user.FirstName,
                ID = id,
                LastName = user.LastName,
                Message = string.Empty,
                SelectedRole = roles.FirstOrDefault(a => a.ID == user.RoleID)?.Name,
                Roles = roles.Select(a => new SelectListItem(a.Name, a.ID.ToString())).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult AttemptUpdate(EditUserModel model)
        {
            var result = _userManager.UpdateUser(new Users
            {
                ID = model.ID,
                FirstName = model.FirstName,
                LastName =  model.LastName,
                RoleID = Guid.Parse(model.SelectedRole)
            });

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully edited user" : "Failed to edit user"});
        }
    }
}