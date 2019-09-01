using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DMTP.lib.dal.Databases;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Enums;
using DMTP.lib.Handlers;
using DMTP.lib.Managers;
using DMTP.lib.ML.Base;
using DMTP.lib.Options;

using DMTP.Worker.Common;
using DMTP.Worker.Objects;

using NLog;

namespace DMTP.Worker.BackgroundWorkers
{
    public class JobWorker
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private Workers _worker;

        private Config _config;

        public async Task<bool> Run(Workers worker, Config config)
        {
            _worker = worker;

            _config = config;

            var workerHandler = new WorkerHandler(_config.WebServiceURL);

            var work = await workerHandler.GetWorkAsync(_worker.Name);

            if (work == null)
            {
                Log.Debug($"No work or connection issues to {_config.WebServiceURL}, waiting until next interval");

                System.Threading.Thread.Sleep(Constants.LOOP_INTERVAL_MS);

                return false;
            }

            work.Started = true;
            work.StartTime = DateTime.Now;

            var result = await workerHandler.UpdateWorkAsync(work);

            if (!result)
            {
                System.Threading.Thread.Sleep(Constants.LOOP_INTERVAL_MS);

                return false;
            }

            if (!Directory.Exists(work.TrainingDataPath))
            {
                work.Completed = true;
                work.Debug = $"Path ({work.TrainingDataPath}) does not exist";
                work.CompletedTime = DateTime.Now;

                result = await workerHandler.UpdateWorkAsync(work);

                if (!result)
                {
                    AddToPending(work);
                }

                return false;
            }

            var options = new TrainerCommandLineOptions
            {
                FolderOfData = work.TrainingDataPath,
                LogLevel = LogLevels.DEBUG
            };

            var featureExtractor = Assembly.Load(work.FeatureExtractorBytes);

            if (featureExtractor == null)
            {
                work.Debug = "Feature Extractor Assembly was not piped to the worker";

                return false;
            }

            var extractor = featureExtractor.GetTypes()
                .Where(a => a.BaseType == typeof(BasePrediction) && !a.IsAbstract)
                .Select(a => ((BasePrediction) Activator.CreateInstance(a)))
                .FirstOrDefault(a => a.MODEL_NAME == work.ModelType);

            if (extractor == null)
            {
                work.Debug = $"Failed to load {work.ModelType} from piped in assembly";

                return false;
            }

            var (outputFile, metrics) = extractor.TrainModel(options);

            if (File.Exists(outputFile))
            {
                work.Model = File.ReadAllBytes(outputFile);
            }

            work.ModelEvaluationMetrics = metrics;
            work.Completed = true;
            work.CompletedTime = DateTime.Now;

            result = await workerHandler.UpdateWorkAsync(work);

            if (result)
            {
                Log.Debug($"{work.ID}.{work.Name} - was successfully trained and saved to {outputFile}");

                Console.WriteLine($"Successfully trained model and saved to {outputFile}");
            } else
            {
                AddToPending(work);
            }

            return result;
        }

        private void AddToPending(Jobs work)
        {
            var db = new LiteDBDatabase();
            var cache = new InMemoryCache();

            var dbManager = new DatabaseManager(db, cache);

            new SubmissionManager(dbManager).AddOfflineSubmission(work);

            Log.Debug($"{work.ID} has been added to the pending submission database");
        }
    }
}