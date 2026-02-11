using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.Database.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        this.userManager = userManager;
    }

    public async Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Enumerable.Empty<UserLookupModel>();
        }

        return await this.userManager.Users
            .Where(u => u.Email!.Contains(query) || u.UserName!.Contains(query))
            .Select(u => new UserLookupModel
            {
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!,
            })
            .Take(10)
            .ToListAsync();
    }
}
