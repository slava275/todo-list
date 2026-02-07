using TodoListShared.Models.Models;

namespace TodoListApp.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query);
}
