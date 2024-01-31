using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;
// have to add better error messages
public class Customer
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Id")]
    [Range(1000, 9999)]
    [Required]
    public int CustomerID { get; set; }


    [StringLength(50)]
    [Required]
    public string Name { get; set; }

    [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "N/A")]
    [RegularExpression("^[0-9]{3} [0-9]{3} [0-9]{3}$", ErrorMessage = "Please follow the format XXX XXX XXX where X is a digit")]
    public string TFN { get; set; }

    [StringLength(50)]
    [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "N/A")]

    public string Address { get; set; }

    [StringLength(40)]
    [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "N/A")]

    public string City { get; set; }

    [RegularExpression("^VIC|NSW|ACT|NT|TAS|WA|QLD|SA$", ErrorMessage = "Please enter a valid state")]
    [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "N/A")]
    public string State { get; set; }



    [RegularExpression("^[0-9]{4}$", ErrorMessage = "Please follow the format XXXX where X is a digit")]
    [Display(Name = "Post Code")]
    [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "N/A")]

    public string PostCode { get; set; }

    [DisplayFormat(ConvertEmptyStringToNull = true, NullDisplayText = "N/A")]
    [RegularExpression("^04[0-9]{2} [0-9]{3} [0-9]{3}$", ErrorMessage = "Please follow the format 04XX XXX XXX where X is a digit")]
    public string Mobile { get; set; }

    [Required]
    public bool Locked { get; set; }
}

