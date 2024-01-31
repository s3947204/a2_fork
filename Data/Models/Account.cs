using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Data.Models;
public class Account
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [Range(1000, 9999)]
    [Required]
    [Key]
    public int AccountNumber { get; set; }

    [Display(Name = "Account Type")]
    [Column(TypeName = "char")]
    [StringLength(1)]
    [RegularExpression("^C|S$")]
    [Required]
    public string AccountType { get; set; }


    public int CustomerID { get; set; }
    public Customer Customer { get; set; }
}

