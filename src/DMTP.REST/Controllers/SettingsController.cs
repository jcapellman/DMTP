using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.REST.Models.Settings;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        protected override AccessSections CurrentSection => AccessSections.SETTINGS;

        public SettingsController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index(string actionMessage = null)
        {
            var model = new SettingsDashboardModel
            {
                ActionMessage = actionMessage,
                Setting = Database.GetSettings()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AttemptUpdate(Settings setting)
        {
            var result = Database.UpdateSettings(setting);

            return RedirectToAction("Index",
                new {actionMessage = result ? "Successfully updated settings" : "Failed to update settings"});
        }
    }
}