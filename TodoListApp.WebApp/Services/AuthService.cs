using System.Text.Json;
using System.Text.Json.Serialization;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Interfaces;
using TodoListApp.WebApp.Models;

namespace TodoListApp.WebApp.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient client;
    private readonly IHttpContextAccessor httpContextAccessor;

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public AuthService(HttpClient client, IHttpContextAccessor httpContext)
    {
        this.client = client;
        this.httpContextAccessor = httpContext;
    }

    public async Task<bool> LoginAsync(LoginViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.client.PostAsJsonAsync("account/login", model, this.options);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<TokenResponseModel>(this.options);

        if (result != null && !string.IsNullOrEmpty(result.Token))
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(3),
                Secure = true,
            };

            this.httpContextAccessor.HttpContext?.Response.Cookies.Append("jwtToken", result.Token, cookieOptions);
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        this.httpContextAccessor.HttpContext?.Response.Cookies.Delete("jwtToken");
    }

    public async Task<bool> RegisterAsync(RegisterViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.client.PostAsJsonAsync("account/register", model, this.options);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VerifyEmail(VerifyEmailViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.client.PostAsJsonAsync("account/verifyEmail", model, this.options);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ResetPasswordAsync(ChangePasswordViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        var response = await this.client.PostAsJsonAsync("account/resetPassword", model, this.options);
        return response.IsSuccessStatusCode;
    }
}
