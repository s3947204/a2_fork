using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Transaction
{
    [Required]
    [Display(Name = "Id")]
    public int TransactionID { get; set; }

    [RegularExpression("^D|W|T|S|B$")]
    [Column(TypeName = "char(1)")]
    [Required]
    [Display(Name = "Type")]

    public string TransactionType { get; set; }

    [ForeignKey("Account")]
    [Required]
    [Display(Name = "Account")]

    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    [ForeignKey("DestinationAccount")]
    [Display(Name = "Destination")]

    [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "N/A")]

    public int? DestinationAccountNumber { get; set; }
    public Account DestinationAccount { get; set; }

    [Column(TypeName = "money")]
    [Required]
    [DataType(DataType.Currency)]

    public decimal Amount { get; set; }

    [StringLength(30)]
    [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "N/A")]
    public string Comment { get; set; }

    [Required]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
    [Display(Name = "Date")]

    public DateTime TransactionTimeUtc { get; set; }
}
