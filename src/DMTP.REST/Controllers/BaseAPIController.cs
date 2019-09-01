using DMTP.lib.dal.Databases.Tables;
using DMTP.lib.dal.Manager;

using DMTP.REST.Filters;

using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(APIAuthorize))]
    [ApiController]
    public class BaseAPIController : BaseController
    {
        protected BaseAPIController(DatabaseManager database, Settings settings) : base(database, settings)
        {
        }
    }
}