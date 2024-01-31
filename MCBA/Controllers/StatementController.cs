using Data.Models;
using MCBA.Services;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Data;

namespace MCBA.Controllers;
public class StatementController : Controller
{
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;


    private readonly MCBAContext _context;
    private readonly AccountServices _accountServices;
    public StatementController(MCBAContext context, AccountServices accountServices)
    {
        _context = context;
        _accountServices = accountServices;
    }


    public async Task<IActionResult> ShowStatement(int accountNumber, int page = 1)
    {
        var accountBelongsToCustomer = await _accountServices.AccountBelongsToCustomer(CustomerID, accountNumber);
        if (!accountBelongsToCustomer)
            return RedirectToAction("Index", "Customer");


        const int pageSize = 4;


        var statementViewModel = new StatementViewModel();

        var account = await _context.Account.FindAsync(accountNumber);

        var accountViewModel = new AccountViewModel()
        {
            Account = account,
            Balance = await _accountServices.GetBalance(accountNumber)
        };

        statementViewModel.AccountViewModel = accountViewModel;

        var transactions = await _context.Transaction.Where(x => x.AccountNumber == accountNumber)
            .OrderByDescending(x => x.TransactionTimeUtc).ToPagedListAsync(page, pageSize);


        statementViewModel.Transactions = transactions;

        return View(statementViewModel);
    }
}
