using System;
using System.Linq;

using DMTP.lib.Databases.Base;
using DMTP.REST.Models.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IDatabase database) : base(database)
        {
        }

        public IActionResult Index(string actionMessage = null)
        {
            var model = new UserDashboardModel
            {
                UserLoginListing = Database.GetLogins().OrderByDescending(a => a.Timestamp).ToList()
            };

            var jobs = Database.GetJobs();

            model.UsersListing = Database.GetUsers().Select(a => new UserListingItem
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                EmailAddress = a.EmailAddress,
                ID = a.ID,
                LastLogin = model.UserLoginListing.Where(b => b.UserID == a.ID).Max(b => b.Timestamp),
                NumJobs = jobs.Count(b => b.SubmittedByUserID == a.ID)
            }).ToList();
    
           if (!string.IsNullOrEmpty(actionMessage))
           {
               model.ActionMessage = actionMessage;
           }

           return View(model);
        }

        [HttpGet]
        public IActionResult DeleteUser(Guid id)
        {
            var result = Database.DeleteUser(id);

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully deleted user" : "Failed to delete user"});
        }

        [HttpGet]
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
        public IActionResult AttemptUpdate(EditUserModel model)
        {
            var result = false;

            return RedirectToAction("Index", new { actionMessage = result ? "Successfully edited user" : "Failed to edit user"});
        }
    }
}