using FinalProject_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace FinalProject_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebAPIService _webAPIService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IWebAPIService webAPIService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _webAPIService = webAPIService;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet("search")]
        public IActionResult Search()
        {
            return View();
        }

        [Authorize]
        [HttpGet("my-booking")]
        public IActionResult MyBooking()
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");

            var response = httpClient.GetAsync("api/booking/get-booking-by-user").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            if (responseString == "[]")
            {
                ViewBag.JsonData = "[]";
            }
            else
            {
                ViewBag.JsonData = responseString;
            }
            ViewBag.JsonData = responseString;

            return View();
        }
    }
}
