using Data.Models;
using MCBA.Services;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Data;

namespace MCBA.Controllers;
public class CustomerController : Controller
{

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    private readonly MCBAContext _context;
    private readonly AccountServices _accountServices;

    public CustomerController(MCBAContext context, AccountServices accountServices)
    {
        _context = context;
        _accountServices = accountServices;
    }

    public async Task<IActionResult> Index()
    {
        var accounts = await _accountServices.GetAccounts(CustomerID);
        var accountViewModels = new List<AccountViewModel>();
        foreach (var account in accounts)
        {
            accountViewModels.Add(new AccountViewModel()
            {
                Account = account,
                Balance = await _accountServices.GetBalance(account.AccountNumber)
            });
        }

        return View(accountViewModels);
    }



}

