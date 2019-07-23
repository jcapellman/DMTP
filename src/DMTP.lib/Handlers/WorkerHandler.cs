using System.Threading.Tasks;

using DMTP.lib.Databases.Tables;
using DMTP.lib.Handlers.Base;

namespace DMTP.lib.Handlers
{
    public class WorkerHandler : BaseHandler
    {
        public WorkerHandler(string rootUrl) : base(rootUrl) { }

        protected override string RootAPI => "Worker";

        public async Task<Jobs> GetWorkAsync(string hostName) => await GetAsync<Jobs>($"hostName={hostName}");

        public async Task<bool> UpdateWorkAsync(Jobs job) => await PostAsync(job);
    }
}