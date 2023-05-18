using FinalProject_MVC.Models;
using FinalProject_MVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Net.Http;
using NuGet.Protocol;
using FinalProject_MVC.Libs;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace FinalProject_MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IWebAPIService _webAPIService;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(
            ILogger<AuthController> logger,
            IWebAPIService webAPIService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _webAPIService = webAPIService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            //check claim is authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            //access token from another web 
            //request to get

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _webAPIService.AuthenticateAsync(model.Email, model.Password);

            if (result["token"] == null)
            {
                TempData["ErrorMessage"] = result["message"].ToJson();
                return RedirectToAction("Login");
            }

            //get token from response
            var accessToken = result["token"].ToString();

            if (!string.IsNullOrEmpty(accessToken))
            {
                // Lưu trữ Access Token dưới dạng Cookie
                var claims = new List<Claim>
                                        {
                                            new Claim(ClaimTypes.Email, model.Email),
                                            new Claim(ClaimTypes.Role, result["role"].ToString()),
                                            new Claim(ClaimTypes.Name, result["username"].ToString()),
                                            new Claim("access_token", accessToken)
                                        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                //if admin redirect to admin page, check by claim
                if (result["role"].ToString() == "manager")
                {
                    //area admin
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }


                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return NotFound();
            }
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _webAPIService.RegisterAsync(model);

            if (result["code"].ToJson() != "200")
            {
                TempData["ErrorMessage"] = result["message"].ToJson();
                return RedirectToAction("Register");
            }
            TempData["Message"] = result["message"].ToJson();
            return RedirectToAction("Register");
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            //delete cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet("notfound")]
        public IActionResult PageNotFound()
        {
            return View();
        }



        [Authorize]
        [HttpGet("get-account")]
        public IActionResult GetAccount()
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var response = httpClient.GetAsync("api/auth/get-account").Result;
            return Ok(JObject.Parse(response.Content.ReadAsStringAsync().Result).ToJson());
        }
    }
}