using Microsoft.AspNetCore.Mvc;

namespace FrontSite.Controllers
{
    public class ManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
