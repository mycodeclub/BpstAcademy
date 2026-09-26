using System.ComponentModel.DataAnnotations;

namespace BpstEdu.Web.Areas.Account.Models;

public sealed class LoginInput
{
    [Required(ErrorMessage = "Enter your email or BPST ID.")]
    [Display(Name = "Email or ID")]
    public string LoginId { get; set; } = "";

    [Required(ErrorMessage = "Enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Keep me signed in on this device")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
