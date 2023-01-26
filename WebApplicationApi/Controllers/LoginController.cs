using DotNet.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json.Nodes;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    public class LoginController : Controller
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();
        //cookie =  HttpCookie("DateCookieExample");

          //myCookie = new HttpCookie(CookieName);
        //CookiesHelper.SetCookie("keyValue", "exampleValue", DateTime.Now.AddDays(30));
        //var value = CookiesHelper.GetCookieValue("keyValue");

        public LoginController()
        {
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logs(string Email, string Password)
        {
            using (var httpClient = new HttpClient(_clientHandler))
            {
                Login body = new Login()
                {
                    email = Email,
                    password = Password,
                    
                };
                StringContent bodyJson = new StringContent(JsonConvert.SerializeObject(body),Encoding.UTF8,"application/json");
                using (var response = await httpClient.PostAsync("https://movies-api-hpwt.onrender.com/connexion", bodyJson))
                {
                    
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        dynamic convertObj = JObject.Parse(apiResponse);

                        string token = convertObj.token;
                        //TempData["success"] = "La connexion est réussie";
                        CookieOptions cookieOptions = new CookieOptions();
                        cookieOptions.Expires = DateTime.Now.AddHours(2);
                        Response.Cookies.Append("TOKEN",token,cookieOptions);
                        return RedirectToAction("Index", "Subscription");
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        dynamic convertObj = JObject.Parse(apiResponse);

                        string message = convertObj.message;
                        TempData["error"] = message;
                        return RedirectToAction("Index");
                    }
                    
                }

                //ViewData.Model = _oFilms;
            }  
            
        }
    }


}
