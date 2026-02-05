using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
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
        public async Task<IActionResult> LoginAsync(string username, string password)
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
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.token);
                var claims = jwt.Claims.ToList();
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Movies");

            }
            return RedirectToAction("Index"); 
        }
        [HttpGet]
        public async Task<IActionResult> LogoutAsync(string username, string password)
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
        private List<Claim> GetUserClaims(string token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwt = handler.ReadJwtToken(token.Replace("Bearer ", ""));
            //foreach(var cl in jwt.Claims.ToList())
            //{
            //    System.Diagnostics.Debug.WriteLine(cl.Type);
            //}
            return jwt.Claims.ToList();

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
