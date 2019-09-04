using System;
using System.IO;

using DMTP.Worker.Objects;

using Newtonsoft.Json;

namespace DMTP.Worker.Helpers
{
    public class ConfigManager
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        public static Config ReadConfig(string configFilename)
        {
            try
            {
                var configString = File.ReadAllText(configFilename);

                if (string.IsNullOrEmpty(configString))
                {
                    throw new NullReferenceException($"{configFilename} was empty or null");
                }

                return JsonConvert.DeserializeObject<Config>(configString);
            }
            catch (Exception ex)
            {
                Log.Error($"Failure reading {configFilename} due to {ex}");

                return null;
            }
        }

        public static bool WriteConfig(Config config, string configFilename)
        {
            try
            {
                var json = JsonConvert.SerializeObject(config);

                File.WriteAllText(configFilename, json);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Failure writing {configFilename} due to {ex}");
                
                return false;
            }
        }
    }
}