using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApp.Interfaces;

public interface ITodoListWebApiService
{
    Task<IEnumerable<TodoListModel>> GetAllAsync();

    Task CreateAsync(TodoListModel model);

    Task UpdateAsync(TodoListModel model);

    Task DeleteByIdAsync(int id);

    Task<TodoListModel> GetByIdAsync(int id);

    Task AddMemberAsync(int todoListId, string userId);

    Task RemoveMemberAsync(int todoListId, string memberId);

    Task UpdateMemberRoleAsync(int todoListId, string memberId, TodoListRole newRole);
}
