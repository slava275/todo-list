using System.ComponentModel.DataAnnotations;

namespace TodoListShared.Models.Models;

public class VerifyEmailViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; }
}
