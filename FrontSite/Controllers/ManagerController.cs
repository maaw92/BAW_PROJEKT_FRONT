using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace FrontSite.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private RestClient _client;
        private string apiUrl = "https://localhost:7163/api/movies";
        public ManagerController()
        {
            _client = new RestClient(apiUrl);
            //_client.AddDefaultHeader("Authorization", $"Bearer {token}");
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public IActionResult Index(MovieEventDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("token") ?? "";
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Index", "Login");
                        
            RestRequest request = new RestRequest($"create", Method.Post);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(new { MovieName = model.Name, MovieDate = model.Date});
            var response = _client.Execute(request);
            if (!response.IsSuccessful)
                TempData["Error"] = "Błąd.";
            else
                TempData["Success"] = "Utworzono zdarzenie.";

            return RedirectToAction(nameof(Index));
        }
        
    }
}
