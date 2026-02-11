using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.Database.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query);
}
