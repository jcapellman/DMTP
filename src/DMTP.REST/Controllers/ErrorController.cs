using DMTP.lib.dal.Databases.Base;
using DMTP.lib.dal.Databases.Tables;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class ErrorController : BaseController
    {
        public ErrorController(IDatabase database, Settings settings) : base(database, settings)
        {
        }

        public IActionResult Index(string errorMessage) => View("Index", errorMessage);
    }
}