using Data.Models;
using System.ComponentModel.DataAnnotations;

namespace MCBA.ViewModel;

public class AccountViewModel
{
    public Account Account { get; set; }

    [DataType(DataType.Currency)]
    public decimal Balance { get; set; }



}

