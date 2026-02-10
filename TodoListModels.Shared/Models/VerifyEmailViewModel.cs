using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Models;

public class VerifyEmailViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; }
}
