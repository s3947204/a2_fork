using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.CustomAttributes;

namespace Data.Models;


// can add custom data annotaion to amount
// to check if its greater than 0.01
// and to check it does not have more than 2 decimal places

public class BillPay
{
    [Key]
    [Required]
    public int BillPayID { get; set; }

    [ForeignKey("Account")]
    [Display(Name = "Account Number")]

    public int AccountNumber { get; set; }
    public Account Account { get; set; }


    [Display(Name = "Payee Name")]

    public int PayeeID { get; set; }
    public Payee Payee { get; set; }


    [Required]
    [Column(TypeName = "money")]
    [DataType(DataType.Currency)]
    [MoreThanTwoDecimalPlaces(ErrorMessage = "Amount cannot have more than two decimal places")]
    [LessThanOneCent(ErrorMessage = "Amount cannot be less than $0.01")]
    public decimal Amount { get; set; }


    [Required]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
    [Display(Name = "Scheduled Time")]
    [DateInFutureToCurrent(ErrorMessage = "Please schedule a time in the future")]
    public DateTime ScheduleTimeUTC { get; set; }

    [Required]
    [RegularExpression("^O|M$", ErrorMessage = "Please enter a valid period")]
    [Column(TypeName = "char")]
    public string Period { get; set; }


    [Required]
    [RegularExpression("^F|P|C$")]
    public string Status { get; set; }

}


