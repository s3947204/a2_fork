using Data.Models;

namespace MCBA.ViewModel;
public class BillPayViewModel
{

    public List<Account> Accounts { get; set; }
    public List<Payee> Payees { get; set; }

    public BillPay BillPay { get; set; }
}

