﻿using System;
using System.Reflection;
using DMTP.lib.Databases.Tables;
using DMTP.Worker.BackgroundWorkers;
using DMTP.Worker.Common;

namespace DMTP.Worker
{
    public class Worker
    {
        private readonly string _serverURL;

        private readonly Workers _worker;

        private readonly CheckinWorker _cWorker = new CheckinWorker();
        private readonly JobWorker _jWorker = new JobWorker();

        public Worker(string serverURL)
        {
            _serverURL = serverURL;

            _worker = new Workers
            {
                Name = Environment.MachineName,
                NumCores = Environment.ProcessorCount,
                WorkerVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                OSVersion = Environment.OSVersion.VersionString
            };
        }

        public async void RunAsync()
        {
            _cWorker.Run(_worker, _serverURL);
            
            while (true)
            {
                var workerResult = await _jWorker.Run(_worker, _serverURL);

                System.Threading.Thread.Sleep(!workerResult
                    ? Constants.LOOP_ERROR_INTERVAL_MS
                    : Constants.LOOP_INTERVAL_MS);
            }
        }
    }
}