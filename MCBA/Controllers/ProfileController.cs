using Data.Models;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using SimpleHashing.Net;
using Data;

namespace MCBA.Controllers;
public class ProfileController : Controller
{
    private static readonly ISimpleHash _simpleHash = new SimpleHash();
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
    private readonly MCBAContext _context;


    public ProfileController(MCBAContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        return View(await GetCustomer(CustomerID));
    }

    public async Task<IActionResult> EditProfile()
    {
        return View(await GetCustomer(CustomerID));
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(Customer customer)
    {



        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("EditProfileFailed", "Please enter valid values");
            return View(customer);
        }


        customer.CustomerID = CustomerID;
        _context.Update(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public IActionResult EditPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EditPassword(ChangePasswordViewModel changePasswordViewModel)
    {
        var login = _context.Login.Where(x => x.CustomerID == CustomerID).First();

        if (!_simpleHash.Verify(changePasswordViewModel.OldPassword, login.PasswordHash))
        {
            ModelState.AddModelError("EditPasswordFailed", "Old password is incorrect");
            return View(changePasswordViewModel);
        }


        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("EditPasswordFailed", "Please enter valid values");
            return View(changePasswordViewModel);
        }

        login.PasswordHash = _simpleHash.Compute(changePasswordViewModel.NewPassword);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Customer");
    }

    private async Task<Customer> GetCustomer(int CustomerID)
    {
        var customer = await _context.Customer.FindAsync(CustomerID);
        return customer;
    }
}
