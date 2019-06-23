using System.Threading.Tasks;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Handlers.Base;

namespace DMTP.lib.Handlers
{
    public class HostsHandler : BaseHandler
    {
        public HostsHandler(string rootUrl) : base(rootUrl) { }
    
        public async Task<bool> AddUpdateHostAsync(Hosts host) => await PostAsync<Hosts>("Hosts", host);
    }
}