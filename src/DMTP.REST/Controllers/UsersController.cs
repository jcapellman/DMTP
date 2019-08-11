using System;
using System.Linq;

using DMTP.lib.Databases.Base;
using DMTP.REST.Models;

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
           var model = new UserListingModel {
                UsersListing = Database.GetUsers(),
                UserLoginListing = Database.GetLogins().OrderByDescending(a => a.Timestamp).ToList()
           };

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
    }
}