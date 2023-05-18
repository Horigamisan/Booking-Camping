using FinalProject_API.Models;
using FinalProject_MVC.Models;
using FinalProject_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;

namespace FinalProject_MVC.Controllers
{
    [Route("booking")]
    public class BookingController : Controller
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IWebAPIService _webAPIService;
        private readonly IHttpClientFactory _httpClientFactory;

        public BookingController(ILogger<BookingController> logger, IWebAPIService webAPIService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _webAPIService = webAPIService;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var response = httpClient.GetAsync("api/booking/get-all-position").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            ViewBag.JsonData = responseString;
            return View();
        }

        [HttpGet("search")]
        public IActionResult Search(SearchModel searchModel)
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            //use post
            var response = httpClient.PostAsync("api/booking/search", new StringContent(JsonConvert.SerializeObject(searchModel), Encoding.UTF8, "application/json")).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;

            var responseGetAllType = httpClient.GetAsync("api/booking/get-all-type").Result;
            var responseStringGetAllType = responseGetAllType.Content.ReadAsStringAsync().Result;

            var responseGetAllServices = httpClient.GetAsync("api/booking/get-all-services").Result;
            var responseStringGetAllServices = responseGetAllServices.Content.ReadAsStringAsync().Result;

            ViewBag.Types = responseStringGetAllType;
            ViewBag.ChiTietSearch = searchModel;
            ViewBag.JsonData = responseString;
            ViewBag.Services = responseStringGetAllServices;

            return View();
        }

        [HttpPost("filter-search")]
        public IActionResult GetFilterSearch(FilterSearchModel filterSearchModel)
        {
            //use get and query string
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            DateTime ngaybatdau = filterSearchModel.Ngaybatdau ?? DateTime.Now;
            DateTime ngayketthuc = filterSearchModel.Ngayketthuc ?? DateTime.Now;

            var response = httpClient.PostAsJsonAsync("api/booking/filter-search", filterSearchModel).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            
            ViewBag.JsonData = responseString;
            ViewBag.Ngaybatdau = ngaybatdau.ToString("dd-MM-yyyy");
            ViewBag.Ngayketthuc = ngayketthuc.ToString("dd-MM-yyyy");
            ViewBag.Soluongnguoi = filterSearchModel.Soluongnguoi;
            return PartialView();
        }

        
        [HttpGet("detail-position/{id}")]
        public IActionResult DetailPosition(int id, string ngaybatdau, string ngayketthuc, int soluongnguoi)
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var response = httpClient.GetAsync("api/booking/get-detail-position/" + id).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            ViewBag.JsonData = responseString;
            ViewBag.Ngaybatdau = ngaybatdau;
            ViewBag.Ngayketthuc = ngayketthuc;
            ViewBag.Soluongnguoi = soluongnguoi;

            return View();
        }

        [Authorize]
        [HttpGet("review-booking")]
        public async Task<IActionResult> ReviewBooking(int id, string ngaybatdau, string ngayketthuc, int soluongnguoi)
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var response = await httpClient.GetAsync("api/booking/get-detail-position/" + id);
            var responseBookingCode = await httpClient.GetAsync("api/booking/get-next-booking-code");
            var responseString = await response.Content.ReadAsStringAsync();

            var responseBookingsString = await responseBookingCode.Content.ReadAsStringAsync();
            var maDatChoCount = JsonConvert.DeserializeObject<int>(responseBookingsString);

            ViewBag.MaDatCho = maDatChoCount;
            ViewBag.JsonData = responseString;
            ViewBag.Ngaybatdau = ngaybatdau;
            ViewBag.Ngayketthuc = ngayketthuc;
            ViewBag.Soluongnguoi = soluongnguoi;
            ViewBag.Songaythue = (DateTime.Parse(ngayketthuc) - DateTime.Parse(ngaybatdau)).Days;
            //claim
            ViewBag.Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            return View();
        }

        [Authorize]
        [HttpPost("booking-camp")]
        public IActionResult BookingCamp(BookingModel bookingModel)
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var response = httpClient.PostAsync("api/booking/booking-camp", new StringContent(JsonConvert.SerializeObject(bookingModel), Encoding.UTF8, "application/json")).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            return Json(responseString);
        }
    }
}
