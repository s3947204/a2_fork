using Data.Models;
using MCBA.Services;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Data;

namespace MCBA.Controllers;


public class TransferController : Controller
{
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;


    private const string _sessionKey_Amount = $"{nameof(TransferController)}_Amount";
    private const string _sessionKey_ChosenAccount = $"{nameof(TransferController)}_ChosenAccount";
    private const string _sessionKey_DestinationAccount = $"{nameof(TransferController)}_DestinationAccount";
    private const string _sessionKey_Comment = $"{nameof(TransferController)}_Comment";


    private readonly MCBAContext _context;
    private readonly AccountServices _accountServices;
    public TransferController(MCBAContext context, AccountServices accountServices)
    {
        _context = context;
        _accountServices = accountServices;
    }


    public async Task<IActionResult> Transfer()
    {
        var accounts = await _accountServices.GetAccounts(CustomerID);
        var transferViewModel = GetTransferViewModel();
        transferViewModel.Accounts = accounts;
        return View(transferViewModel);

    }

    [HttpPost]
    public async Task<IActionResult> Transfer(TransferViewModel transferViewModel)
    {
        var chosenAccount = transferViewModel.AccountNumber;
        var destinationAccount = transferViewModel.DestinationAccountNumber;
        var amount = transferViewModel.Amount;
        var comment = transferViewModel.Comment;
        var sufficientFunds = true;

        transferViewModel.Accounts = await _accountServices.GetAccounts(CustomerID);

        var chosenAccountBelongsToCustomer = await _accountServices.AccountBelongsToCustomer(CustomerID, chosenAccount);
        var destinationAccountExists = await _accountServices.DoesAccountExist(destinationAccount);

        if (chosenAccountBelongsToCustomer)
            sufficientFunds = await _accountServices.SufficientFunds(chosenAccount, amount, "T");

        if (!chosenAccountBelongsToCustomer)
            ModelState.AddModelError("AccountNumber", "Please choose a valid account to transfer from");

        if (!destinationAccountExists)
            ModelState.AddModelError("DestinationAccountNumber", "Destination account does not exist");

        if (destinationAccount == chosenAccount)
            ModelState.AddModelError("TransferFailed", "You cannot transfer to the same account");

        if (!sufficientFunds)
            ModelState.AddModelError("Amount", $"Insufficient funds(Balance: ${(await _accountServices.GetBalance(chosenAccount)):F2})");




        if (!ModelState.IsValid)
            return View(transferViewModel);



        HttpContext.Session.SetString(_sessionKey_ChosenAccount, chosenAccount.ToString());
        HttpContext.Session.SetString(_sessionKey_DestinationAccount, destinationAccount.ToString());
        HttpContext.Session.SetString(_sessionKey_Amount, amount.ToString());

        if (comment != null)
            HttpContext.Session.SetString(_sessionKey_Comment, comment);


        return RedirectToAction(nameof(TransferConfirm));
    }


    public IActionResult TransferConfirm()
    {
        var chosenAccount = HttpContext.Session.GetString(_sessionKey_ChosenAccount);
        if (chosenAccount == null)
            return RedirectToAction(nameof(Transfer));

        return View(GetTransferViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> TransferConfirmPost()
    {
        var transferViewModel = GetTransferViewModel();
        var transferTransaction = new Transaction()
        {
            TransactionType = "T",
            Amount = transferViewModel.Amount,
            Comment = transferViewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = transferViewModel.AccountNumber,
            DestinationAccountNumber = transferViewModel.DestinationAccountNumber
        };

        var transferTrasactionReceiver = new Transaction()
        {
            TransactionType = "T",
            Amount = transferViewModel.Amount,
            Comment = transferViewModel.Comment,
            TransactionTimeUtc = DateTime.UtcNow,
            AccountNumber = transferViewModel.DestinationAccountNumber
        };


        _context.Transaction.Add(transferTransaction);
        _context.Transaction.Add(transferTrasactionReceiver);
        if (await _accountServices.FreeTransactionsConsumed(transferViewModel.AccountNumber))
        {
            var serviceTransaction = new Transaction()
            {
                TransactionType = "S",
                Amount = 0.10M,
                Comment = "Service fee",
                TransactionTimeUtc = DateTime.UtcNow,
                AccountNumber = transferViewModel.AccountNumber
            };
            _context.Transaction.Add(serviceTransaction);
        }



        HttpContext.Session.Remove(_sessionKey_Comment);
        HttpContext.Session.Remove(_sessionKey_Amount);
        HttpContext.Session.Remove(_sessionKey_ChosenAccount);
        HttpContext.Session.Remove(_sessionKey_DestinationAccount);

        await _context.SaveChangesAsync();
        return Redirect("/Customer");
    }


    private TransferViewModel GetTransferViewModel()
    {
        TransferViewModel transferViewModel = new();

        if (int.TryParse(HttpContext.Session.GetString(_sessionKey_ChosenAccount), out int chosenAccount))
        {
            transferViewModel.AccountNumber = chosenAccount;
        }

        if (int.TryParse(HttpContext.Session.GetString(_sessionKey_DestinationAccount), out int destinationAccount))
        {
            transferViewModel.DestinationAccountNumber = destinationAccount;
        }


        if (decimal.TryParse(HttpContext.Session.GetString(_sessionKey_Amount), out decimal amount))
        {
            transferViewModel.Amount = amount;
        }

        transferViewModel.Comment = HttpContext.Session.GetString(_sessionKey_Comment);

        return transferViewModel;
    }
}

