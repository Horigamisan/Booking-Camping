using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FinalProject_MVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "manager")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
