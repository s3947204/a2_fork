using Data.Models;
using Microsoft.EntityFrameworkCore;
using Data;
namespace MCBA.Services;

public class BillPayBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BillPayBackgroundService> _logger;

    public BillPayBackgroundService(IServiceProvider services, ILogger<BillPayBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Bill pay Background Service is running.");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception has occured");
            }


            _logger.LogInformation("Bill pay Background Service is waiting a minute.");

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MCBAContext>();
        var accountServices = scope.ServiceProvider.GetRequiredService<AccountServices>();
        var currentTime = DateTime.UtcNow;
        // get all billpays that are pending, and have a date that is higher than the current date
        var billPays = await context.BillPay.Where(x => x.Status == "P" && (DateTime.Compare(x.ScheduleTimeUTC, currentTime) < 0)).ToListAsync(cancellationToken);
        foreach (BillPay billpay in billPays)
        {
            if (await accountServices.SufficientFunds(billpay.AccountNumber, billpay.Amount, "B"))
            {
                var transaction = new Transaction()
                {
                    TransactionType = "B",
                    AccountNumber = billpay.AccountNumber,
                    Amount = billpay.Amount,
                    Comment = "Bill pay",
                    TransactionTimeUtc = DateTime.UtcNow
                };
                context.Transaction.Add(transaction);
                if (billpay.Period == "O")
                    context.BillPay.Remove(billpay);
                else
                    billpay.ScheduleTimeUTC = billpay.ScheduleTimeUTC.AddMonths(1);
            }
            else
                billpay.Status = "F";

            await context.SaveChangesAsync();
        }
    }
}

