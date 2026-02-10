using System.ComponentModel.DataAnnotations;

namespace TodoListApp.WebApi.Models.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Електронна пошта обов'язкова")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обов'язковий")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має бути від 8 до 100 символів")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження пароля")]
    [Compare("Password", ErrorMessage = "Паролі не збігаються")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
