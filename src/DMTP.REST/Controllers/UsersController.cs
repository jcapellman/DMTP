using System;
using System.Linq;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Enums;
using DMTP.lib.dal.Manager;

using DMTP.lib.Managers;
using DMTP.lib.Security;

using DMTP.REST.Attributes;
using DMTP.REST.Models.Users;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.Extensions.Localization;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly UserManager _userManager;

        private readonly IStringLocalizer<UsersController> _localizer;

        public UsersController(DatabaseManager database, Settings settings, IStringLocalizer<UsersController> localizer) : base(database, settings)
        {
            _userManager = new UserManager(Database);

            _localizer = localizer;
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
            SendEmail(email, _localizer["SendEmailSubject"], _localizer["SendEmailBody"]);

            return RedirectToAction("Index", "Users", new {actionMessage = $"{_localizer["EmailSentMessage"]}{email}"});
        }

        [HttpGet]
        [Access(AccessSections.USERS, AccessLevels.FULL)]
        public IActionResult DeleteUser(Guid id)
        {
            var result = _userManager.DeleteUser(id);

            return RedirectToAction("Index", new { actionMessage = result ? _localizer["SuccessfullyDeletedUser"] : _localizer["FailedToDeleteUser"]});
        }

        [HttpGet]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult Edit(Guid id)
        {
            var user = _userManager.GetUser(id);

            var roles = new RoleManager(Database).GetRoles();

            var model = new CreateEditUserModel
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
        public IActionResult AttemptUpdate(CreateEditUserModel model)
        {
            var result = _userManager.UpdateUser(new Users
            {
                ID = model.ID.Value,
                FirstName = model.FirstName,
                LastName =  model.LastName,
                RoleID = Guid.Parse(model.SelectedRole)
            });

            return RedirectToAction("Index", new { actionMessage = result ? _localizer["SuccessfullyEditedUser"] : _localizer["FailToEditUser"]});
        }

        [HttpGet]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult Create() => View(new CreateEditUserModel
        {
            Roles = new RoleManager(Database).GetRoles().Select(a => new SelectListItem(a.Name, a.ID.ToString())).ToList()
        });

        [HttpPost]
        [Access(AccessSections.USERS, AccessLevels.EDIT)]
        public IActionResult AttemptCreate(CreateEditUserModel model)
        {
            var result = _userManager.CreateUser(model.EmailAddress, model.FirstName, model.LastName,
                model.Password.ToSHA1(), Guid.Parse(model.SelectedRole));

            return RedirectToAction("Index", new { actionMessage = result.HasValue ? _localizer["SuccessfullyCreatedUser"] : _localizer["FailToEditUser"]});
        }
    }
}