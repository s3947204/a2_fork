using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public class Login
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Login Id")]
    [RegularExpression("^[0-9]{8}$", ErrorMessage ="Please ensure the login id is vaild")]
    [StringLength(8, MinimumLength = 8)]
    [Column(TypeName = "char")]
    [Required]
    public string LoginID { get; set; }

    public int CustomerID { get; set; }
    public Customer Customer { get; set; }

    [StringLength(94, MinimumLength = 94)]
    [Column(TypeName = "char")]
    [Required]
    public string PasswordHash { get; set; }




}

