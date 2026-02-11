using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Interfaces;

public interface IUserWebApiService
{
    Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query);
}
