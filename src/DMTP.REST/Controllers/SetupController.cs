using System.Linq;

using DMTP.lib.Common;
using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Helpers;
using DMTP.REST.Models.Setup;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class SetupController : BaseController
    {
        public SetupController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index(SetupModel model = null)
        {
            if (CurrentSettings.IsInitialized)
            {
                return RedirectToAction("Index", "Account");
            }

            Database.Initialize();

            return View(model ?? new SetupModel());
        }

        public IActionResult AttemptSetup(SetupModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ActionMessage = "Ensure all fields are filled out";

                return RedirectToAction("Index", new {model = model});
            }

            CurrentSettings.IsInitialized = true;
            CurrentSettings.AllowNewUserCreation = model.AllowUserCreation;
            CurrentSettings.SMTPHostName = model.SMTPHostName;
            CurrentSettings.SMTPPassword = model.SMTPPassword;
            CurrentSettings.SMTPPortNumber = model.SMTPPortNumber;
            CurrentSettings.SMTPUsername = model.SMTPUsername;

            Database.UpdateSettings(CurrentSettings);

            var adminRole = Database.GetRoles().FirstOrDefault(a => a.Name == Constants.ROLE_BUILTIN_ADMIN);

            if (adminRole == null)
            {
                model.ActionMessage = "Admin Role could not be found";

                return RedirectToAction("Index", model);
            }

            var user = Database.CreateUser(model.EmailAddress, model.FirstName, model.LastName, model.Password.ToSHA1(), adminRole.ID);

            if (user != null)
            {
                return Login(user.Value, model.EmailAddress);
            }

            model.ActionMessage = "Failed to create user";

            return RedirectToAction("Index", model);
        }
    }
}