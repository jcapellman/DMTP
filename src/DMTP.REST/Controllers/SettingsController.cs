using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Helpers;
using DMTP.lib.Managers;

using DMTP.REST.Models.Settings;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly SettingManager _settingManager;

        public SettingsController(DatabaseManager database, Settings settings) : base(database, settings)
        {
            _settingManager = new SettingManager(database);
        }

        public IActionResult Index(string actionMessage = null)
        {
            var model = new SettingsDashboardModel
            {
                ActionMessage = actionMessage,
                Setting = _settingManager.GetSettings()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AttemptUpdate(SettingsDashboardModel model)
        {
            if (model.GenerateNewRegistrationKey)
            {
                model.Setting.DeviceKeyPassword = Strings.GenerateRandomString();
            }

            var result = _settingManager.UpdateSettings(model.Setting);

            if (result)
            {
                CurrentSettings = model.Setting;
            }

            return RedirectToAction("Index",
                new {actionMessage = result ? "Successfully updated settings" : "Failed to update settings"});
        }
    }
}