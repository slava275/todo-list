using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface IUserWebApiService
{
    Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query);
}
