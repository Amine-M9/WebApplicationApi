using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using WebApplicationApi.Models;
using static System.Net.WebRequestMethods;

namespace WebApplicationApi.Controllers
{

    public class SubscriptionController : Controller
    {
        public static List<Films> _oFilms { get; set; }

        HttpClientHandler _clientHandler = new HttpClientHandler();

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var token = Request.Cookies["TOKEN"];

            using (var httpClient = new HttpClient(_clientHandler))
            {
               
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                using (var response = await httpClient.GetAsync("https://movies-api-hpwt.onrender.com/subscriptions"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    _oFilms = JsonConvert.DeserializeObject<List<Films>>(apiResponse);

                }

                ViewData.Model = _oFilms;
            }
            return View();
        }

    }
}
