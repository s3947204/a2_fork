using AdminWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AdminWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Login()
        {
            var username = HttpContext.Session.GetString(nameof(AdminWeb.Models.Login.Username));
            if (username != null)
                return RedirectToAction("Index", "LoggedIn");
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login login)
        {
            if(login.Username != "admin" || login.Password != "admin")
            {
                ModelState.AddModelError("LoginFailed", "Login failed, please try again.");

                return View(login);
            }
            // Login customer.
            HttpContext.Session.SetString(nameof(AdminWeb.Models.Login.Username), login.Username);

            return RedirectToAction("Index", "LoggedIn");

        }


        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
