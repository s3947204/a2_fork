using Data.Models;
using Microsoft.EntityFrameworkCore;
using Data;
namespace MCBA.Services;

public class AccountServices
{
    private readonly MCBAContext _context;
    public AccountServices(MCBAContext context)
    {
        _context = context;
    }


    /// <summary>
    /// Given an account number, it returns the balance of that account by using its transactions.
    /// It assumes that the account number exists so it is up to the caller to ensure tha it exists
    /// </summary>
    /// <param name="AccountNumber">The account number for the account</param>
    /// <returns>The balance for the given account</returns>
    public async Task<decimal> GetBalance(int AccountNumber)
    {
        var transactions = await _context.Transaction.Where(x => x.AccountNumber == AccountNumber).ToListAsync();
        var balance = decimal.Zero;
        foreach (var transaction in transactions)
        {
            if (transaction.TransactionType == "D" || (transaction.TransactionType == "T" && transaction.DestinationAccountNumber == null))
            {
                balance = decimal.Add(transaction.Amount, balance);
            }
            else
            {
                balance = decimal.Subtract(balance, transaction.Amount);
            }

        }
        return balance;
    }


    /// <summary>
    /// Given an accountNumber, it checks if an account with that accountNumber exists
    /// </summary>
    /// <param name="accountNumber"></param>
    /// <returns>True if exists, false otherwise</returns>
    public async Task<bool> DoesAccountExist(int accountNumber)
    {
        var account = await _context.Account.FindAsync(accountNumber);
        if (account == null)
            return false;
        return true;
    }


    /// <summary>
    /// Given a customerID, the method returns all the accounts that belongs to that customer
    /// </summary>
    /// <param name="CustomerID">The given customer id</param>
    /// <returns>All accounts that belong that customer</returns>

    public async Task<List<Account>> GetAccounts(int customerID)
    {
        var accounts = await _context.Account.Where(x => x.CustomerID == customerID).ToListAsync();
        return accounts;
    }

    /// <summary>
    /// Checks if the account associated with the given account number belongs to the customer associated with the given customer id
    /// </summary>
    /// <param name="customerID">Used to identify customer</param>
    /// <param name="accountNumber">Used to identify account</param>
    /// <returns>True if it belongs to the custoemr, false otherwise</returns>
    public async Task<bool> AccountBelongsToCustomer(int customerID, int accountNumber)
    {
        var accountIsCustomers = false;
        var accounts = await GetAccounts(customerID);
        foreach (Account account in accounts)
        {
            if (account.AccountNumber == accountNumber)
                accountIsCustomers = true;
        }

        return accountIsCustomers;
    }



    /// <summary>
    /// Given an account number, it will determine if the account associated with that account number has consumed its free transactions
    /// Free transactions implying, no service fee charged. 
    /// </summary>
    /// <param name="accountNumber">The given account number</param>
    /// <returns>True if consumed, false otherwise</returns>
    public async Task<bool> FreeTransactionsConsumed(int accountNumber)
    {
        var transactions = await _context.Transaction.Where(x => x.AccountNumber == accountNumber).ToListAsync();
        var feeChargingTransaction = 0;
        foreach (Transaction transaction in transactions)
        {
            if (transaction.TransactionType == "W" || (transaction.TransactionType == "T" && transaction.DestinationAccountNumber != null))
            {
                feeChargingTransaction++;
            }
        }
        return feeChargingTransaction >= 2;
    }

    /// <summary>
    /// Determines if a account has enough balance to perform certain transactions
    /// </summary>
    /// <param name="accountNumber">The accountNumber of the account</param>
    /// <param name="amount">The amount that will be processed</param>
    /// <param name="transactionType">The type of transactin, used to determine service fee amount</param>
    /// <returns>True if sufficient funds are present, false otherwise</returns>
    public async Task<bool> SufficientFunds(int accountNumber, decimal amount, string transactionType)
    {

        var chargeServiceFee = await FreeTransactionsConsumed(accountNumber);
        var balance = await GetBalance(accountNumber);
        var serviceFeeAmount = 0M;
        var compareValue = 300M;

        var account = await _context.Account.FindAsync(accountNumber);
        if (account.AccountType == "S")
            compareValue = 0M;

        if (transactionType == "W")
            serviceFeeAmount = 0.05M;
        if (transactionType == "T")
            serviceFeeAmount = 0.10M;

        if (chargeServiceFee)
            balance = decimal.Subtract(balance, serviceFeeAmount);

        balance = decimal.Subtract(balance, amount);

        return decimal.Compare(balance, compareValue) >= 0;
    }
}

