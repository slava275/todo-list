using System.ComponentModel.DataAnnotations;

namespace TodoListShared.Models.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email обов'язковий")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль обов'язковий")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Запам'ятати мене")]
    public bool RememberMe { get; set; }
}
