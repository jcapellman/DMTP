using DMTP.lib.Databases.Base;
using DMTP.REST.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        public UsersController(IDatabase database) : base(database)
        {
        }

        public IActionResult Index()
        {
            var model = new UserListingModel
            {
                UsersListing = Database.GetUsers(),
                UserLoginListing = Database.GetLogins()
            };

            return View(model);
        }
    }
}