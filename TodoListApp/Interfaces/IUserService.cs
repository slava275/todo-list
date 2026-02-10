using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query);
}
