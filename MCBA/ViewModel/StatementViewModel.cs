using Data.Models;
using X.PagedList;

namespace MCBA.ViewModel;
public class StatementViewModel
{
    public AccountViewModel AccountViewModel { get; set; }
    public IPagedList<Transaction> Transactions { get; set; }
}

