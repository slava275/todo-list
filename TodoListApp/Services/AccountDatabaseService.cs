using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TodoList.Data.Entities;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class AccountDatabaseService : IAccountService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IConfiguration config;

    public AccountDatabaseService(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        this.userManager = userManager;
        this.config = config;
    }

    public async Task<string> LoginAsync(LoginViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var user = await this.userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return null;
        }

        var isPasswordValid = await this.userManager.CheckPasswordAsync(user, model.Password);

        if (!isPasswordValid)
        {
            return null;
        }

        return this.GenerateJwtToken(user);
    }

    public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
        };

        return await this.userManager.CreateAsync(user, model.Password);
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(this.config["Jwt:Key"] !));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email !),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email !),
        };

        var token = new JwtSecurityToken(
            issuer: this.config["Jwt:Issuer"],
            audience: this.config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
