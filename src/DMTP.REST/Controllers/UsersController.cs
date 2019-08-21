using System;
using System.Linq;

using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.REST.Attributes;
using DMTP.REST.Models.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        [Access(AccessSections.USERS, AccessLevels.VIEW_ONLY)]
        public IActionResult Index(string actionMessage = null)
        {
            var model = new UserDashboardModel(GetApplicationUser())
            {
                UserLoginListing = Database.GetLogins().OrderByDescending(a => a.Timestamp).ToList(),
            };

            var jobs = Database.GetJobs();

            model.UsersListing = Database.GetUsers().Select(a => new UserListingItem
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
            var result = Database.DeleteUser(id);

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully deleted user" : "Failed to delete user"});
        }

        [HttpGet]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult Edit(Guid id)
        {
            var user = Database.GetUser(id);

            var model = new EditUserModel
            {
                FirstName = user.FirstName,
                ID = id,
                LastName = user.LastName,
                Message = string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult AttemptUpdate(EditUserModel model)
        {
            var result = Database.UpdateUser(new Users
            {
                ID = model.ID,
                FirstName = model.FirstName,
                LastName =  model.LastName
            });

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully edited user" : "Failed to edit user"});
        }
    }
}