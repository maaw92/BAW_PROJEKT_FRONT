using System.Diagnostics;
using FrontSite.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {            
            return RedirectToAction("Index", "Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
