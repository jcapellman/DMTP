using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Helpers;
using DMTP.lib.Managers;

using DMTP.REST.Models.Settings;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class SettingsController : BaseController
    {
        private readonly SettingManager _settingManager;

        private readonly IStringLocalizer<SettingsController> _localizer;

        public SettingsController(DatabaseManager database, Settings settings, IStringLocalizer<SettingsController> localizer) : base(database, settings)
        {
            _settingManager = new SettingManager(database);

            _localizer = localizer;
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
                new {actionMessage = result ? _localizer["SuccessfullyUpdated"] : _localizer["FailedUpdated"]});
        }
    }
}