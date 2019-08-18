using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.REST.Models.Setup;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    public class SetupController : BaseController
    {
        protected override AccessSections CurrentSection => AccessSections.SETUP;

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

            var user = Database.CreateUser(model.EmailAddress, model.FirstName, model.LastName, model.Password);

            if (user == null)
            {
                model.ActionMessage = "Failed to create user";

                return RedirectToAction("Index", model);
            }

            // TODO: Update Settings
            
            return Login(user.Value);
        }
    }
}