using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net;

namespace FrontSite.Controllers
{
    [Authorize(Roles = "User,Manager")]
    public class MoviesController : Controller
    {
        private RestClient _client;
        private string apiUrl = "https://localhost:7163/api/movies";
        public MoviesController()
        {
            _client = new RestClient(apiUrl);
            //_client.AddDefaultHeader("Authorization", $"Bearer {token}");
        }
        private bool IsTokenNull(string token)
        {
            return string.IsNullOrWhiteSpace(token);
        }
        public IActionResult Index()
        {            
            string token = HttpContext.Session.GetString("token") ?? "";
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Index", "Login");
            RestRequest request = new RestRequest("/get");
            request.Method = Method.Get;
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = _client.Execute<List<MovieEventDto>>(request);            
            if (!response.IsSuccessful)
            {
                if(response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    return RedirectToAction("Index", "Login");
                return RedirectToAction("Error", "Home");
            }
            return View(response.Data);
        }
        public IActionResult MyReservations()
        {
            string token = HttpContext.Session.GetString("token") ?? "";
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Index", "Login");
            RestRequest request = new RestRequest("/getExistingReservations");
            request.Method = Method.Get;
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = _client.Execute<List<ExistingEventDto>>(request);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    return RedirectToAction("Index", "Login");
                return RedirectToAction("Error", "Home");
            }
            return View(response.Data);
        }

        public IActionResult Seats(int id)
        {
            string token = HttpContext.Session.GetString("token") ?? "";
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Index", "Login");
            RestRequest request = new RestRequest($"/{id}/seats", Method.Get);
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = _client.Execute<List<SeatDto>>(request);
            if (!response.IsSuccessful)
            {
                if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
                    return RedirectToAction("Index", "Login");
                return RedirectToAction("Error", "Home");
            }
            ViewBag.EventId = id;
            return View(response.Data);
        }

        [HttpPost]
        public IActionResult Reserve(int eventId, int seatId)
        {
            var token = HttpContext.Session.GetString("token") ?? "";
            if (string.IsNullOrWhiteSpace(token)) return RedirectToAction("Index", "Login");
            RestRequest request = new RestRequest($"/{eventId}/reserve", Method.Post);            
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(new { SeatId = seatId });
            var response = _client.Execute(request);
            if (!response.IsSuccessful)
                TempData["Error"] = "Błąd. Spróbuj ponownie.";
            else
                TempData["Success"] = "Rezerwacja udana.";
            return RedirectToAction("Seats", new { id = eventId });
        }
    }
    public class MovieEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
    public class ExistingEventDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string SeatNo { get; set; }
    }

    public class SeatDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; }
    }
}
