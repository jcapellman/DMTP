using System;
using System.IO;

using DMTP.Worker.Common;
using DMTP.Worker.Helpers;
using DMTP.Worker.Objects;

namespace DMTP.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var configExists = false;

            Config config = null;

            if (!File.Exists(Constants.CONFIG_FILE))
            {
                Console.WriteLine($"{Constants.CONFIG_FILE} does not exist");
            }
            else
            {
                configExists = true;

                config = ConfigManager.ReadConfig(Constants.CONFIG_FILE);
            }

            if (args.Length != 2 && !configExists)
            {
                Console.WriteLine($"{Constants.CONFIG_FILE} does not exist and not arguments passed in, exiting");

                return;
            }

            if (args.Length == 2)
            {
                config = new Config
                {
                    RegistrationKey = args[0],
                    DebugLogLevel = false,
                    WebServiceURL = args[1]
                };

                ConfigManager.WriteConfig(config, Constants.CONFIG_FILE);
            }

            if (config == null)
            {
                Console.WriteLine("Config was null, exiting");

                return;
            }

            new Worker(config).RunAsync();
        }
    }
}