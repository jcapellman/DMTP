using DMTP.lib.Databases.Base;
using DMTP.lib.Databases.Tables;
using DMTP.lib.Enums;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseAPIController : BaseController
    {
        protected override AccessSections CurrentSection => AccessSections.API;

        protected BaseAPIController(IDatabase database, Settings settings) : base(database, settings)
        {
        }
    }
}