using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DMTP.lib.Databases.Tables;
using DMTP.lib.Handlers;

namespace DMTP.Uploader
{
    class Program
    {
        private const int NUM_REQUIRED_ARGUMENTS = 4;

        private static (string Url, Jobs Job) ParseCommandLineArgs(IReadOnlyList<string> args)
        {
            // Eventually force the use of a json file
            if (args.Count != NUM_REQUIRED_ARGUMENTS)
            {
                Console.WriteLine($"Usage is:{Environment.NewLine}DMTP.Uploader <name> <model type> <path to data> <server url>");

                return (null, null);
            }

            return (args[3], new Jobs
            {
                ModelType = args[1],
                TrainingDataPath = args[2],
                Name = args[0]
            });
        }

        static async Task Main(string[] args)
        {
            var (url, job) = ParseCommandLineArgs(args);
            
            if (job == null || url == null)
            {
                Console.WriteLine("Failed to parse arguments - exiting");

                return;
            }

            var jobHandler = new JobHandler(url);

            var result = await jobHandler.AddNewJobAsync(job);

            Console.WriteLine(result ? "Job successfully uploaded" : "Failed to upload job");
        }
    }
}