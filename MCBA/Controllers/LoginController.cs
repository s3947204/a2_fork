using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleHashing.Net;
using Data;

namespace MCBA.Controllers;
[AllowAnonymous]

public class LoginController : Controller
{
    private static readonly ISimpleHash _simpleHash = new SimpleHash();
    private readonly MCBAContext _context;

    public LoginController(MCBAContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        var customerID = HttpContext.Session.GetInt32(nameof(Customer.CustomerID));
        if (customerID.HasValue)
            return RedirectToAction("Index", "Customer");
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Login(string loginID, string password)
    {
        var login = await _context.Login.Include(x => x.Customer).FirstOrDefaultAsync(x => x.LoginID == loginID);
        if (login == null || string.IsNullOrEmpty(password) || !_simpleHash.Verify(password, login.PasswordHash))
            ModelState.AddModelError("LoginFailed", "Login failed, please try again.");
       
        if(login != null)
        {
            if (login.Customer.Locked)
                ModelState.AddModelError("LoginFailed", "You have been locked out of your account");
        }

        if (!ModelState.IsValid)
            return View(new Login { LoginID = loginID });

        // Login customer.
        HttpContext.Session.SetInt32(nameof(Customer.CustomerID), login.CustomerID);
        HttpContext.Session.SetString(nameof(Customer.Name), login.Customer.Name);

        return RedirectToAction("Index", "Customer");
    }


    public IActionResult Logout()
    {
        // Logout customer.
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

}

