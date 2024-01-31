using Data.Models;
using MCBA.Services;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Data;

namespace MCBA.Controllers;
public class WithdrawController : Controller
{
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    private const string _sessionKey_Amount = $"{nameof(WithdrawController)}_Amount";
    private const string _sessionKey_ChosenAccount = $"{nameof(WithdrawController)}_ChosenAccount";
    private const string _sessionKey_Comment = $"{nameof(WithdrawController)}_Comment";


    private readonly MCBAContext _context;
    private readonly AccountServices _accountServices;
    public WithdrawController(MCBAContext context, AccountServices accountServices)
    {
        _context = context;
        _accountServices = accountServices;
    }

    public async Task<IActionResult> Withdraw()
    {
        var accounts = await _accountServices.GetAccounts(CustomerID);

        var withdrawViewModel = GetWithdrawViewModel();
        withdrawViewModel.Accounts = accounts;
        return View(withdrawViewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Withdraw(WithdrawViewModel withdrawViewModel)
    {
        var chosenAccount = withdrawViewModel.ChosenAccount;
        var amount = withdrawViewModel.Amount;
        var comment = withdrawViewModel.Comment;
        withdrawViewModel.Accounts = await _accountServices.GetAccounts(CustomerID);
        var sufficientFunds = true;

        var accountBelongsToCustomer = await _accountServices.AccountBelongsToCustomer(CustomerID, chosenAccount);
        if (accountBelongsToCustomer)
            sufficientFunds = await _accountServices.SufficientFunds(chosenAccount, amount, "W");


        if (!accountBelongsToCustomer)
            ModelState.AddModelError("ChosenAccount", "Invalid value");
        if (!sufficientFunds)
            ModelState.AddModelError("Amount", $"Insufficient funds(Balance: ${(await _accountServices.GetBalance(chosenAccount)):F2})");


        if (!ModelState.IsValid)
            return View(withdrawViewModel);



        HttpContext.Session.SetString(_sessionKey_ChosenAccount, chosenAccount.ToString());
        HttpContext.Session.SetString(_sessionKey_Amount, amount.ToString());

        if (comment != null)
            HttpContext.Session.SetString(_sessionKey_Comment, comment);


        return RedirectToAction(nameof(WithdrawConfirm));
    }

    public IActionResult WithdrawConfirm()
    {
        var chosenAccount = HttpContext.Session.GetString(_sessionKey_ChosenAccount);
        if (chosenAccount == null)
            return RedirectToAction(nameof(Withdraw));

        return View(GetWithdrawViewModel());
    }


    [HttpPost]
    public async Task<IActionResult> WithdrawConfirmPost()
    {
        var withdrawViewModel = GetWithdrawViewModel();
        var withdrawTransaction = new Transaction()
        {
            TransactionType = "W",
            Amount = withdrawViewModel.Amount,
            Comment = withdrawViewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = withdrawViewModel.ChosenAccount
        };

        _context.Transaction.Add(withdrawTransaction);
        if (await _accountServices.FreeTransactionsConsumed(withdrawViewModel.ChosenAccount))
        {
            var serviceTransaction = new Transaction()
            {
                TransactionType = "S",
                Amount = 0.05M,
                Comment = "Service fee",
                TransactionTimeUtc = DateTime.UtcNow,
                AccountNumber = withdrawViewModel.ChosenAccount
            };
            _context.Transaction.Add(serviceTransaction);
        }



        HttpContext.Session.Remove(_sessionKey_Comment);
        HttpContext.Session.Remove(_sessionKey_Amount);
        HttpContext.Session.Remove(_sessionKey_ChosenAccount);

        await _context.SaveChangesAsync();
        return Redirect("/Customer");
    }






    private WithdrawViewModel GetWithdrawViewModel()
    {
        WithdrawViewModel withdrawViewModel = new();

        if (int.TryParse(HttpContext.Session.GetString(_sessionKey_ChosenAccount), out int chosenAccount))
        {
            withdrawViewModel.ChosenAccount = chosenAccount;
        }

        if (decimal.TryParse(HttpContext.Session.GetString(_sessionKey_Amount), out decimal amount))
        {
            withdrawViewModel.Amount = amount;
        }

        withdrawViewModel.Comment = HttpContext.Session.GetString(_sessionKey_Comment);

        return withdrawViewModel;
    }
}