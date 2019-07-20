using System;
using System.Threading.Tasks;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.lib.Handlers;

namespace DMTP.Uploader
{
    class Program
    {
        private static (string Url, Jobs Job) ParseCommandLineArgs(string[] args)
        {
            // Eventually force the use of a json file
            if (args.Length < 4)
            {
                Console.WriteLine($"Usage is:{Environment.NewLine}FileClassifier.JobManager.Uploader <name> <model type> <path to data> <server url>");

                return (null, null);
            }

            if (!Enum.TryParse<ModelType>(args[1], true, out _))
            {
                Console.WriteLine($"Invalid Model Type option ({args[1]})");

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
            var (Url, Job) = ParseCommandLineArgs(args);
            
            if (Job == null || Url == null)
            {
                return;
            }

            var jobHandler = new JobHandler(Url);

            var result = await jobHandler.AddNewJobAsync(Job);

            Console.WriteLine(result ? "Job successfully uploaded" : "Failed to upload job");
        }
    }
}