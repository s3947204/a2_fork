using System.ComponentModel.DataAnnotations;

namespace MCBA.ViewModel;
public class ChangePasswordViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Must be between 5 to 100 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "Old Password")]

    public string OldPassword { get; set; }


    [Required]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Must be between 5 to 100 characters")]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }
}

