using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Helpers;
using DMTP.REST.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        public AccountController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index(string ReturnUrl = "")
        {
            if (!CurrentSettings.IsInitialized)
            {
                return RedirectToAction("Index", "Setup");
            }

            return View(new LoginViewModel
            {
                CurrentSettings = CurrentSettings
            });
        }

        public IActionResult Create()
        {
            if (!CurrentSettings.AllowNewUserCreation)
            {
                return RedirectToAction("Index", "Account");
            }

            return View(new CreateUserModel());
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();

            return RedirectToAction("Index", "Account");
        }

        [HttpPost]
        public IActionResult AttemptCreate(CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please try again";
                
                return View("Create", model);
            }

            var userGuid = Database.CreateUser(model.EmailAddress, model.FirstName, model.LastName, model.Password.ToSHA1(), model.RoleID);

            Database.RecordLogin(userGuid, model.EmailAddress, Request.HttpContext.Connection.RemoteIpAddress.ToString(), userGuid.HasValue);

            if (userGuid != null)
            {
                return Login(userGuid.Value);
            }

            model.ErrorMessage = "EmailAddress already exists, try again";

            model.Password = string.Empty;
            model.EmailAddress = string.Empty;
            model.FirstName = string.Empty;
            model.LastName = string.Empty;

            return View("Create", model);
        }

        [HttpPost]
        public IActionResult AttemptLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ErrorMessage = "Please try again";

                model.CurrentSettings = CurrentSettings;

                return View("Index", model);
            }

            var userGuid = Database.GetUser(model.EmailAddress, model.Password.ToSHA1());

            Database.RecordLogin(userGuid, model.EmailAddress, Request.HttpContext.Connection.RemoteIpAddress.ToString(), userGuid.HasValue);

            if (userGuid != null)
            {
                return Login(userGuid.Value);
            }

            model.ErrorMessage = "Email Address and or Password are incorrect";
            model.CurrentSettings = CurrentSettings;

            return View("Index", model);
        }
    }
}