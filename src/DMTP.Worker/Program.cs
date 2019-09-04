using System.IO;

using DMTP.Worker.Common;
using DMTP.Worker.Helpers;
using DMTP.Worker.Objects;

using NLog;

namespace DMTP.Worker
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var configExists = false;

            Config config = null;

            if (!File.Exists(Constants.CONFIG_FILE))
            {
                Log.Warn($"{Constants.CONFIG_FILE} does not exist, attempting to read command line arguments");
            }
            else
            {
                configExists = true;

                Log.Info($"{Constants.CONFIG_FILE} exists, attempting to read");

                config = ConfigManager.ReadConfig(Constants.CONFIG_FILE);

                if (config == null)
                {
                    Log.Error($"Failed to parse {Constants.CONFIG_FILE}, exiting");

                    return;
                }
            }

            if (args.Length != 2 && !configExists)
            {
                Log.Error($"{Constants.CONFIG_FILE} does not exist and no/invalid arguments passed in, exiting");

                return;
            }

            if (args.Length == 2)
            {
                config = new Config
                {
                    RegistrationKey = args[1],
                    DebugLogLevel = false,
                    WebServiceURL = args[0]
                };

                ConfigManager.WriteConfig(config, Constants.CONFIG_FILE);

                Log.Debug($"{Constants.CONFIG_FILE} was initialized with the command line arguments and written to disk");
            }

            if (config == null)
            {
                Log.Error("Failed to initialize Config, exiting");

                return;
            }

            new Worker(config).RunAsync();
        }
    }
}