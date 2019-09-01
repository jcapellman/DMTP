using System;
using System.IO;

using DMTP.Worker.Objects;

using Newtonsoft.Json;

namespace DMTP.Worker.Helpers
{
    public class ConfigManager
    {
        public static Config ReadConfig(string configFilename)
        {
            var configString = File.ReadAllText(configFilename);

            if (string.IsNullOrEmpty(configString))
            {
                throw new NullReferenceException($"{configFilename} was empty or null");
            }

            return JsonConvert.DeserializeObject<Config>(configString);
        }

        public static bool WriteConfig(Config config, string configFilename)
        {
            var json = JsonConvert.SerializeObject(config);

            File.WriteAllText(configFilename, json);

            return true;
        }
    }
}