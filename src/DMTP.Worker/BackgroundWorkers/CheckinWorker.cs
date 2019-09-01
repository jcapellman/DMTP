using System.ComponentModel;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Handlers;

using DMTP.Worker.Common;
using DMTP.Worker.Objects;

using NLog;

namespace DMTP.Worker.BackgroundWorkers
{
    public class CheckinWorker
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BackgroundWorker _bwCheckin;

        private Workers _worker;

        private Config _config;

        public CheckinWorker()
        {
            _bwCheckin = new BackgroundWorker();

            _bwCheckin.DoWork += BwCheckin_DoWork;
            _bwCheckin.RunWorkerCompleted += BwCheckin_RunWorkerCompleted;            
        }

        public void Run(Workers worker, Config config)
        {
            _worker = worker;
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
            var hostHandler = new WorkerHandler(_config.WebServiceURL, _config.RegistrationKey);

            // Call to checkin with the server
            var checkinResult = await hostHandler.AddUpdateWorkerAsync(_worker);

            if (checkinResult)
            {
                return;
            }

            Log.Error($"Failed to check in with {_config.WebServiceURL}");

            System.Threading.Thread.Sleep(Constants.LOOP_ERROR_INTERVAL_MS);
        }
    }
}