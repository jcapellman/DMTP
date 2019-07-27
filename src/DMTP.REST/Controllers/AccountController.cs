using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMTP.REST.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}