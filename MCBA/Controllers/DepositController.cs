using Data.Models;
using MCBA.Services;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Data;

namespace MCBA.Controllers;
public class DepositController : Controller
{
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    private const string _sessionKey_Amount = $"{nameof(DepositController)}_Amount";
    private const string _sessionKey_ChosenAccount = $"{nameof(DepositController)}_ChosenAccount";
    private const string _sessionKey_Comment = $"{nameof(DepositController)}_Comment";


    private readonly MCBAContext _context;
    private readonly AccountServices _accountServices;
    public DepositController(MCBAContext context, AccountServices accountServices)
    {
        _context = context;
        _accountServices = accountServices;
    }

    public async Task<IActionResult> Deposit()
    {
        var accounts = await _accountServices.GetAccounts(CustomerID);

        var depositViewModel = GetDepositViewModel();
        depositViewModel.Accounts = accounts;
        return View(depositViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Deposit(DepositViewModel depositViewModel)
    {
        var chosenAccount = depositViewModel.ChosenAccount;
        var amount = depositViewModel.Amount;
        var comment = depositViewModel.Comment;
        depositViewModel.Accounts = await _accountServices.GetAccounts(CustomerID);


        var accountBelongsToCustomer = await _accountServices.AccountBelongsToCustomer(CustomerID, chosenAccount);

        if (!accountBelongsToCustomer)
            ModelState.AddModelError("ChosenAccount", "Invalid value");

        if (!ModelState.IsValid)
            return View(depositViewModel);



        HttpContext.Session.SetString(_sessionKey_ChosenAccount, chosenAccount.ToString());
        HttpContext.Session.SetString(_sessionKey_Amount, amount.ToString());

        if (comment != null)
            HttpContext.Session.SetString(_sessionKey_Comment, comment);


        return RedirectToAction(nameof(DepositConfirm));
    }


    public IActionResult DepositConfirm()
    {
        var chosenAccount = HttpContext.Session.GetString(_sessionKey_ChosenAccount);
        if (chosenAccount == null)
            return RedirectToAction(nameof(Deposit));

        return View(GetDepositViewModel());
    }


    [HttpPost]
    public async Task<IActionResult> DepositConfirmPost()
    {
        var depositViewModel = GetDepositViewModel();
        _context.Transaction.Add(new Transaction()
        {
            TransactionType = "D",
            AccountNumber = depositViewModel.ChosenAccount,
            Amount = depositViewModel.Amount,
            Comment = depositViewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow
        });

        HttpContext.Session.Remove(_sessionKey_Comment);
        HttpContext.Session.Remove(_sessionKey_Amount);
        HttpContext.Session.Remove(_sessionKey_ChosenAccount);

        await _context.SaveChangesAsync();
        return Redirect("/Customer");
    }


    /// <summary>
    /// Gets the deposit view model, populates some of the field 
    /// This method can be called when the session variables are not set, hence the TryParse
    /// </summary>
    /// <returns>Deposit view model</returns>
    private DepositViewModel GetDepositViewModel()
    {
        DepositViewModel depositViewModel = new();

        //depositViewModel.ChosenAccount = null; // throws compiler error

        if (int.TryParse(HttpContext.Session.GetString(_sessionKey_ChosenAccount), out int chosenAccount))
        {
            depositViewModel.ChosenAccount = chosenAccount;
        }

        if (decimal.TryParse(HttpContext.Session.GetString(_sessionKey_Amount), out decimal amount))
        {
            depositViewModel.Amount = amount;
        }


        // depositViewModel.Comment = null // valid code
        // No need to try parse since it can be null
        depositViewModel.Comment = HttpContext.Session.GetString(_sessionKey_Comment);

        return depositViewModel;
    }
}
