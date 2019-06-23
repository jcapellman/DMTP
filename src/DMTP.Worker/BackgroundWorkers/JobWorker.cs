using System;
using System.IO;
using System.Threading.Tasks;
using DMTP.lib.Databases;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;
using DMTP.lib.Handlers;
using DMTP.lib.Options;
using DMTP.Worker.Common;
using NLog;

namespace DMTP.Worker.BackgroundWorkers
{
    public class JobWorker
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private Hosts _host;

        private string _serverURL;

        public async Task<bool> Run(Hosts host, string serverURL)
        {
            _host = host;

            _serverURL = serverURL;

            var workerHandler = new WorkerHandler(_serverURL);

            var work = await workerHandler.GetWorkAsync(_host.Name);

            if (work == null)
            {
                Log.Debug($"No work or connection issues to {_serverURL}, waiting until next interval");

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

            var (outputFile, metrics) = (string.Empty, string.Empty);
            /*
             TODO: Switch to a reflection based approach
            switch (Enum.Parse<ModelType>(work.ModelType, true))
            {
                case ModelType.CLASSIFICATION:
                    (outputFile, metrics) = new ClassificationEngine().TrainModel(options);
                    break;
                case ModelType.CLUSTERING:
                    (outputFile, metrics) = new ClusteringEngine().TrainModel(options);
                    break;
            }
            */
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

            db.AddOfflineSubmission(work);

            Log.Debug($"{work.ID} has been added to the pending submission database");
        }
    }
}