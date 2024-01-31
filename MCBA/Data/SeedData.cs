using Data.Models;
using DTO;
using Newtonsoft.Json;
using Data;
namespace MCBA.Data;

public static class SeedData
{
    private static async Task<string> GetJSON()
    {
        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        // Making the web request
        using var client = new HttpClient();
        string json = await client.GetStringAsync(Url);
        return json;
    }

    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<MCBAContext>();

        Initialize(context);
    }

    public static void Initialize(MCBAContext context)
    {

        // Look for customers.
        if (context.Customer.Any())
            return; // DB has already been seeded.

        var Json = GetJSON();
        var customers = JsonConvert.DeserializeObject<List<CustomerDTO>>(Json.Result, new JsonSerializerSettings
        {
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
        });




        foreach (CustomerDTO customer in customers)
        {
            context.Customer.Add(new Customer
            {
                CustomerID = customer.CustomerID,
                Name = customer.Name,
                Address = customer.Address,
                City = customer.City,
                // if the postcode is null then set it to null otherwise convert to string 
                // is ef core doing the work in the background where it sets the value to DBNull isntead of null
                PostCode = customer.PostCode == null ? null : customer.PostCode.ToString()
            });

            context.Login.Add(new Login
            {
                LoginID = customer.Login.LoginID.ToString(),
                CustomerID = customer.CustomerID,
                PasswordHash = customer.Login.PasswordHash
            });


            foreach (AccountDTO account in customer.Accounts)
            {
                context.Account.Add(new Account
                {
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    CustomerID = customer.CustomerID
                });

                foreach (TransactionDTO transaction in account.Transactions)
                {
                    context.Transaction.Add(new Transaction
                    {
                        TransactionType = "D",
                        AccountNumber = account.AccountNumber,
                        Amount = transaction.Amount,
                        Comment = transaction.Comment,
                        TransactionTimeUtc = transaction.TransactionTimeUtc
                    });
                }
            }
        }

        context.Payee.AddRange(
            new Payee()
            {
                Name = "Optus", 
                Address = "100 fake street", 
                City = "Melbourne", 
                State = "VIC", 
                PostCode = "3000", 
                Mobile = "0432343234"

            },
            new Payee()
            {
                Name = "Apple",
                Address = "100 real street",
                City = "Sydney",
                State = "NSW",
                PostCode = "2000",
                Mobile = "0998092343"
            },
            new Payee()
            {
                Name = "Vodafone",
                Address = "20 flindoff lane",
                City = "Melbourne",
                State = "VIC",
                PostCode = "3000",
                Mobile = "0932123239"
            }
         );


        context.SaveChanges();
    }
}
