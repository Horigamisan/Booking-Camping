using FinalProject_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FinalProject_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "manager")]

    public class DatChoController : Controller
    {
        private readonly ILogger<DatChoController> _logger;
        private readonly IWebAPIService _webAPIService;
        private readonly IHttpClientFactory _httpClientFactory;

        public DatChoController(ILogger<DatChoController> logger, IWebAPIService webAPIService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _webAPIService = webAPIService;
            _httpClientFactory = httpClientFactory;
        }

        // GET: DatChoController
        public ActionResult Index()
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");

            var response = httpClient.GetAsync("api/manage/booking").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            ViewBag.JsonData = responseString;

            return View();
        }

        // GET: DatChoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DatChoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DatChoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DatChoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DatChoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DatChoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DatChoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
