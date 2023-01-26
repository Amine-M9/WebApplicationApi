using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using WebApplicationApi.Models;

namespace WebApplicationApi.Controllers
{
    public class SignUpController : Controller
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();

        public SignUpController()
        {
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string Name,string Email, string Password, string ConfirmPassword)
        {
            if (ConfirmPassword == Password)
            {
            using (var httpClient = new HttpClient(_clientHandler))
            {
                SignUp body = new SignUp()
                {
                    name = Name,
                    email = Email,
                    password = Password,

                };
                StringContent bodyJson = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://movies-api-hpwt.onrender.com/register", bodyJson))
                {

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        dynamic convertObj = JObject.Parse(apiResponse);

                        string token = convertObj.token;
                        CookieOptions cookieOptions = new CookieOptions();
                        cookieOptions.Expires = DateTime.Now.AddHours(2);
                        Response.Cookies.Append("TOKEN", token, cookieOptions);
                        //TempData["success"] = "L'inscription est réussie";
                        return RedirectToAction("Index","Subscription");
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

            }
            }
            else
            {
                TempData["error"] = "Les mots de passes fournis ne se correspondent pas";
                return RedirectToAction("Index");
            }
        }
    }
}
