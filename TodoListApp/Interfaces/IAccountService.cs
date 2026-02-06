using Microsoft.AspNetCore.Identity;
using TodoListShared.Models.Models;

namespace TodoListApp.Interfaces;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterViewModel model);

    Task<string> LoginAsync(LoginViewModel model);

    Task<string?> VerifyEmailAndGenerateToken(VerifyEmailViewModel model);

    Task<IdentityResult> ResetPasswordAsync(ChangePasswordViewModel model);
}
