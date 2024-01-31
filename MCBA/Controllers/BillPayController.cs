using Data.Models;
using MCBA.Services;
using MCBA.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;

namespace MCBA.Controllers;
public class BillPayController : Controller
{
    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;
    private readonly MCBAContext _context;
    private readonly AccountServices _accountServices;

    public BillPayController(MCBAContext context, AccountServices accountServices)
    {
        _context = context;
        _accountServices = accountServices;
    }


    public async Task<IActionResult> Index()
    {
        var billPayList = new List<BillPay>();
        var customerAccounts = await _accountServices.GetAccounts(CustomerID);

        foreach (Account customerAccount in customerAccounts)
        {
            var billPay = await _context.BillPay.Include(x => x.Payee).
                Where(x => x.AccountNumber == customerAccount.AccountNumber).
                OrderByDescending(x => x.ScheduleTimeUTC).ToListAsync();


            billPayList.AddRange(billPay);
        }


        return View(billPayList);
    }

    public async Task<IActionResult> CreateBillPay()
    {
        return View(await GetBillPayViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateBillPay(BillPay billPay)
    {
        var accountBelongsToCustomer = await _accountServices.AccountBelongsToCustomer(CustomerID, billPay.AccountNumber);
        var payeeExists = (await _context.Payee.FindAsync(billPay.PayeeID)) == null ? false : true;


        if (!accountBelongsToCustomer)
            ModelState.AddModelError("BillPay.AccountNumber", "Please choose a valid value");
        if (!payeeExists)
            ModelState.AddModelError("BillPay.PayeeID", "Please choose a valid value");


        if (!ModelState.IsValid)
        {
            var billPayViewModel = await GetBillPayViewModel();
            billPayViewModel.BillPay = billPay;
            return View(billPayViewModel);
        }


        billPay.Status = "P";
        billPay.ScheduleTimeUTC = billPay.ScheduleTimeUTC.ToUniversalTime();
        _context.BillPay.Add(billPay);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Cancel(int id)
    {
        var billPay = await _context.BillPay.FindAsync(id);

        if (!(await ValidateBillPay(billPay, id)))
            return RedirectToAction("Index");

        billPay.Status = "C";
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Retry(int id)
    {
        var billPay = await _context.BillPay.FindAsync(id);

        if (!(await ValidateBillPay(billPay, id)))
            return RedirectToAction("Index");

        billPay.Status = "P";
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        var billPay = await _context.BillPay.FindAsync(id);

        if (!(await ValidateBillPay(billPay, id)))
            return RedirectToAction("Index");

        _context.BillPay.Remove(billPay);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    private async Task<bool> ValidateBillPay(BillPay billPay, int id)
    {

        if (billPay == null)
            return false;

        var valid = false;

        var accounts = await _context.Account
            .Where(x => x.CustomerID == CustomerID)
            .ToListAsync();
        foreach (Account account in accounts)
        {
            if (billPay.AccountNumber == account.AccountNumber)
                valid = true;
        }

        return valid;
    }

    private async Task<BillPayViewModel> GetBillPayViewModel()
    {
        var billPayViewModel = new BillPayViewModel();
        var customerAccounts = await _accountServices.GetAccounts(CustomerID);
        var payees = await _context.Payee.ToListAsync();

        billPayViewModel.Accounts = customerAccounts;
        billPayViewModel.Payees = payees;

        return billPayViewModel;
    }
}
