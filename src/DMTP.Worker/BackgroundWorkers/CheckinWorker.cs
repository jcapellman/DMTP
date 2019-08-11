    using System.ComponentModel;

using DMTP.lib.Databases.Tables;
using DMTP.lib.Handlers;
using DMTP.Worker.Common;

using NLog;

namespace DMTP.Worker.BackgroundWorkers
{
    public class CheckinWorker
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly BackgroundWorker _bwCheckin;

        private Workers _worker;

        private string _serverUrl;

        public CheckinWorker()
        {
            _bwCheckin = new BackgroundWorker();

            _bwCheckin.DoWork += BwCheckin_DoWork;
            _bwCheckin.RunWorkerCompleted += BwCheckin_RunWorkerCompleted;            
        }

        public void Run(Workers worker, string serverUrl)
        {
            _worker = worker;
            _serverUrl = serverUrl;

            _bwCheckin.RunWorkerAsync();
        }

        private void BwCheckin_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Threading.Thread.Sleep(Constants.LOOP_INTERVAL_MS);

            _bwCheckin.RunWorkerAsync();
        }

        private async void BwCheckin_DoWork(object sender, DoWorkEventArgs e)
        {
            var hostHandler = new WorkerHandler(_serverUrl);

            // Call to checkin with the server
            var checkinResult = await hostHandler.AddUpdateWorkerAsync(_worker);

            if (checkinResult)
            {
                return;
            }

            Log.Error($"Failed to check in with {_serverUrl}");;

            System.Threading.Thread.Sleep(Constants.LOOP_ERROR_INTERVAL_MS);
        }
    }
}