using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Text.Json.Serialization;

namespace FrontSite.Controllers
{
    public class LoginController : Controller
    {
        private string authUrl = "https://localhost:7163/api/auth/login";
        public IActionResult Index()
        {
            var test = HttpContext.Session.GetString("token");
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            RestClient client = new RestClient(authUrl);
            RestRequest request = new RestRequest();
            request.Method = Method.Post;
            TokenRequest body = new TokenRequest() { Email = username, Password = password };
            request.AddJsonBody(body);            
            RestResponse response = client.Execute(request);
            if(response.IsSuccessStatusCode)
            {
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(response.Content);
                if (token == null)
                    return RedirectToAction("Index", "Login");
                HttpContext.Session.SetString("token", token.token);
                return RedirectToAction("Index", "Movies");

            }
            return RedirectToAction("Index"); 
        }
        [HttpGet]
        public IActionResult Logout(string username, string password)
        {
            HttpContext.Session.Remove("token");
            //maybe could revoke token here.
            return RedirectToAction("Index");
        }

    }
    public class TokenResponse
    {
        public string token { get; set; }
    }
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
