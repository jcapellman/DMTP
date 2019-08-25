using System.Threading.Tasks;

using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.Handlers.Base;

namespace DMTP.lib.Handlers
{
    public class JobHandler : BaseHandler
    {
        public JobHandler(string rootUrl) : base(rootUrl) { }

        protected override string RootAPI => "Job";

        public async Task<bool> AddNewJobAsync(Jobs job) => await PostAsync(job);
    }
}