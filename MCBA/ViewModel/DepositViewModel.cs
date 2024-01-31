
using Data.Models;
using System.ComponentModel.DataAnnotations;
using Utilities.CustomAttributes;

namespace MCBA.ViewModel;
public class DepositViewModel
{
    public List<Account> Accounts { get; set; }


    [Display(Name = "Choose Account")]
    public int ChosenAccount { get; set; }


    [MoreThanTwoDecimalPlaces(ErrorMessage = "Amount cannot have more than two decimal places")]
    [LessThanOneCent(ErrorMessage = "Amount cannot be less than $0.01")]
    [Required]
    public decimal Amount { get; set; }

    [StringLength(30, ErrorMessage = "Comment cannot be more than 30 characters")]
    public string Comment { get; set; }

}
