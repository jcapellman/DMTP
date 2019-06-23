using System.Threading.Tasks;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Handlers.Base;

namespace DMTP.lib.Handlers
{
    public class JobHandler : BaseHandler
    {
        public JobHandler(string rootUrl) : base(rootUrl) { }
    
        public async Task<bool> AddNewJobAsync(Jobs job) => await PostAsync<Jobs>("Job", job);
    }
}