using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Helpers;
using DMTP.lib.Managers;
using DMTP.lib.Security;
using DMTP.REST.Models.Setup;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DMTP.REST.Controllers
{
    public class SetupController : BaseController
    {
        private readonly SetupManager _setupManager;

        private IStringLocalizer<SetupController> _localizer;

        public SetupController(DatabaseManager database, Settings settings, IStringLocalizer<SetupController> localizer) : base(database, settings)
        {
            _setupManager = new SetupManager(Database);

            _localizer = localizer;
        }

        public IActionResult Index(SetupModel model = null)
        {
            if (CurrentSettings.IsInitialized)
            {
                return RedirectToAction("Index", "Account");
            }

            _setupManager.Initialize();

            return View(model ?? new SetupModel());
        }

        public IActionResult AttemptSetup(SetupModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ActionMessage = _localizer["EnsureAllFieldsAreFilledOut"];

                return RedirectToAction("Index", new {model = model});
            }

            CurrentSettings.IsInitialized = true;
            CurrentSettings.AllowNewUserCreation = model.AllowUserCreation;
            CurrentSettings.SMTPHostName = model.SMTPHostName;
            CurrentSettings.SMTPPassword = model.SMTPPassword;
            CurrentSettings.SMTPPortNumber = model.SMTPPortNumber;
            CurrentSettings.SMTPUsername = model.SMTPUsername;
            CurrentSettings.DeviceKeyPassword = Strings.GenerateRandomString();

            new SettingManager(Database).UpdateSettings(CurrentSettings);

            var adminRole = new RoleManager(Database).GetRoles().FirstOrDefault(a => a.Name == Constants.ROLE_BUILTIN_ADMIN);

            if (adminRole == null)
            {
                model.ActionMessage = _localizer["AdminRoleNotFound"];

                return RedirectToAction("Index", model);
            }

            var user = new UserManager(Database).CreateUser(model.EmailAddress, model.FirstName, model.LastName, model.Password.ToSHA1(), adminRole.ID);

            if (user != null)
            {
                return Login(user.Value, model.EmailAddress);
            }

            model.ActionMessage = _localizer["FailedToCreateUser"];

            return RedirectToAction("Index", model);
        }
    }
}