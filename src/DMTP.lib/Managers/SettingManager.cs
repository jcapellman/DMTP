using System;

using DMTP.lib.Common;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers.Base;

namespace DMTP.lib.Managers
{
    public class SettingManager : BaseManager
    {
        public SettingManager(DatabaseManager database) : base(database)
        {
        }

        public Settings GetSettings()
        {
            try
            {
                var settings = _database.GetOne<Settings>(a => a != null);

                if (settings != null)
                {
                    return settings;
                }

                settings = new Settings
                {
                    AllowNewUserCreation = false,
                    SMTPHostName = string.Empty,
                    SMTPPassword = string.Empty,
                    SMTPPortNumber = Constants.DEFAULT_SMTP_PORT,
                    SMTPUsername = string.Empty,
                    IsInitialized = false
                };

                _database.Insert(settings);

                return settings;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to get settings due to {ex}");

                return null;
            }
        }

        public bool UpdateSettings(Settings setting)
        {
            try
            {
                var result = _database.Update(setting);

                if (!result)
                {
                    _database.Insert(setting);
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to update settings {setting.ID} in the Settings Table to {ex}");

                return false;
            }
        }
    }
}