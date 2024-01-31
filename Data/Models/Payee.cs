using System.ComponentModel.DataAnnotations;

namespace Data.Models;
public class Payee
{
    [Key]
    [Required]

    public int PayeeID { get; set; }

    [StringLength(50)]
    [Required]

    public string Name { get; set; }


    [StringLength(50)]
    [Required]
    public string Address { get; set; }


    [StringLength(40)]
    [Required]
    public string City { get; set; }

    [StringLength(3, MinimumLength = 2)]
    [Required]
    [RegularExpression("^VIC|NSW|ACT|NT|TAS|WA|QLD|SA$", ErrorMessage = "Please enter a valid state")]
    public string State { get; set; }



    [RegularExpression("^[0-9]{4}$", ErrorMessage = "Please follow the format XXXX where X is a digit")]
    [Display(Name = "Post Code")]
    [Required]
    [StringLength(4)]
    public string PostCode { get; set; }


    [RegularExpression("^0[0-9]{1} [0-9]{4} [0-9]{4}$", ErrorMessage = "Please follow the format 04XX XXX XXX where X is a digit")]
    [StringLength(14)]
    [Required]
    public string Mobile { get; set; }


}
