﻿using System;
using System.ComponentModel;
using System.Linq;

using DMTP.lib.dal.Databases;
using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;
using DMTP.lib.Handlers;
using DMTP.lib.Managers;

using DMTP.Worker.Common;
using DMTP.Worker.Objects;

using Newtonsoft.Json;

using NLog;

namespace DMTP.Worker.BackgroundWorkers
{
    public class PendingSubmissionsWorker
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BackgroundWorker _bwCheckin;

        private readonly LiteDBDatabase _db;

        private Config _config;

        public PendingSubmissionsWorker()
        {
            _db = new LiteDBDatabase();

            _bwCheckin = new BackgroundWorker();
            _bwCheckin.DoWork += BwCheckin_DoWork;
            _bwCheckin.RunWorkerCompleted += BwCheckin_RunWorkerCompleted;
        }

        public void Run(Config config)
        {
            _config = config;

            _bwCheckin.RunWorkerAsync();
        }

        private void BwCheckin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Threading.Thread.Sleep(Constants.LOOP_INTERVAL_MS);

            _bwCheckin.RunWorkerAsync();
        }

        private async void BwCheckin_DoWork(object sender, DoWorkEventArgs e)
        {
            var submissionManager = new SubmissionManager(new DatabaseManager(_db, null));

            var pendingJobs = submissionManager.GetPendingSubmissions();

            if (!pendingJobs.Any())
            {
                Log.Debug("No Pending Jobs found");

                return;
            }

            Log.Debug($"{pendingJobs.Count} pending jobs found...");

            var workerHandler = new WorkerHandler(_config.WebServiceURL, _config.RegistrationKey);

            foreach (var pJob in pendingJobs)
            {
                Jobs job = null;

                try
                {
                    job = JsonConvert.DeserializeObject<Jobs>(pJob.JobJSON);
                } catch (Exception ex)
                {
                    Log.Error($"For Job ID {pJob.ID}, could not parse {pJob.JobJSON} into a Jobs object due to {ex}");
                }

                if (job == null)
                {
                    Log.Error($"Job was null - removing {pJob.ID} from Queue");

                    submissionManager.RemoveOfflineSubmission(pJob.ID);

                    continue;
                }

                var result = await workerHandler.UpdateWorkAsync(job);

                if (result)
                {
                    Log.Debug($"{job.ID} was successfully uploaded");

                    submissionManager.RemoveOfflineSubmission(pJob.ID);

                    continue;
                }

                Log.Debug($"{job.ID} was not able to be uploaded - will retry at a later date and time");
            }            
        }
    }
}