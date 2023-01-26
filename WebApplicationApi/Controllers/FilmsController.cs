using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using WebApplicationApi.Models;


namespace WebApplicationApi.Controllers
{
    public class FilmsController : Controller
    {
        public static List<Films> _oFilms { get; set; }
        List<Films> _Details = new List<Films>();

        HttpClientHandler _clientHandler = new HttpClientHandler();

        public FilmsController()
        {
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pg)
        {
            
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    using (var response = await httpClient.GetAsync("https://movies-api-hpwt.onrender.com/allmovies"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        _oFilms = JsonConvert.DeserializeObject<List<Films>>(apiResponse);
                        

                    }
                }
            _oFilms.AddRange(_oFilms);
            _oFilms.AddRange(_oFilms);
            const int pageSize = 8;
                if (pg < 1)
                {
                    pg = 1;
                }
                
                
                int recsCount = _oFilms.Count();
                var pager = new Pagination(recsCount,pg, pageSize);
                int recSkip = (pg - 1) * pageSize;
                var data = _oFilms.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;
            ViewData.Model = data;
            
            return View();
        }


        public ActionResult Index(string searchString)
        {
            var result = _oFilms;
            
           
            if (!string.IsNullOrEmpty(searchString))
            {
                result = result.FindAll(a => a.Name.ToLower().Contains(searchString.ToLower())).ToList();
                ViewData.Model = result;
                return View();
            }
            else
            {
                ViewData.Model = _oFilms;
                return RedirectToAction("Index");
            }
           
    
        }



        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            _Details = new List<Films>();
            using (var httpClient = new HttpClient(_clientHandler))
            {
                using (var response = await httpClient.GetAsync("https://movies-api-hpwt.onrender.com/movie/"+id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    _Details = JsonConvert.DeserializeObject<List<Films>>(apiResponse);

                }
                var token = Request.Cookies["TOKEN"];
                if (token!=null) {

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                    using (var response = await httpClient.GetAsync("https://movies-api-hpwt.onrender.com/subscription/" + id))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            dynamic convertObj = JObject.Parse(apiResponse);

                            string data = convertObj.Subscribed;
                            if (data == "True")
                            {
                                ViewBag.Subscribed = true;
                            }
                            else
                            {
                                ViewBag.Subscribed = false;
                            }
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
               

                ViewData.Model = _Details;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(int id)
        {
            var token = Request.Cookies["TOKEN"];

            using (var httpClient = new HttpClient(_clientHandler))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                using (var response = await httpClient.PostAsync("https://movies-api-hpwt.onrender.com/subscription/" + id, null))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Le film a été ajouté avec succès";
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
                }
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe(int id)
        {
            var token = Request.Cookies["TOKEN"];
            using (var httpClient = new HttpClient(_clientHandler))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {token}");
                using (var response = await httpClient.DeleteAsync("https://movies-api-hpwt.onrender.com/subscription/" + id))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["success"] = "Le film a été supprimé avec succès";
                        return RedirectToAction("Index", "Subscription");
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        dynamic convertObj = JObject.Parse(apiResponse);

                        string message = convertObj.message;
                        TempData["error"] = message;
                        return RedirectToAction("Index", "Subscription");
                    }
                }
            }
        }

        public IActionResult Deconnexion()
        {
            Response.Cookies.Delete("TOKEN");

            return RedirectToAction("Index");
        }

        public ActionResult ScanResults(string tResults)
        {

            var test = tResults;

            if (string.IsNullOrEmpty(tResults))
            {
                ViewData.Model = _oFilms;
            }
            else
            {
                var result = _oFilms.Where(a => a.Name.ToLower().Contains(tResults.ToLower()));
                ViewData.Model = result;
            }

            return RedirectToAction("Index");

        }
    }
}
