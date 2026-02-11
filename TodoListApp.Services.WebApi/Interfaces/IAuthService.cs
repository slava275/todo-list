using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(LoginViewModel model);

    Task<bool> RegisterAsync(RegisterViewModel model);

    Task LogoutAsync();

    Task<bool> ResetPasswordAsync(ChangePasswordViewModel model);

    Task<bool> VerifyEmail(VerifyEmailViewModel model);
}
