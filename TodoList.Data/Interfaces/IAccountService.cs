using Microsoft.AspNetCore.Identity;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.Database.Interfaces;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterViewModel model);

    Task<string> LoginAsync(LoginViewModel model);

    Task<string?> VerifyEmailAndGenerateToken(VerifyEmailViewModel model);

    Task<IdentityResult> ResetPasswordAsync(ChangePasswordViewModel model);
}
