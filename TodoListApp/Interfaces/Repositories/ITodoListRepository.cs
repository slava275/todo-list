using TodoListApp.Services.Database.Entities;

namespace TodoListApp.WebApi.Interfaces.Repositories;

public interface ITodoListRepository
{
    Task<TodoListEntity?> GetByIdAsync(int id);

    Task<TodoListEntity?> GetWithMembersAsync(int id);

    Task<IEnumerable<TodoListEntity>> GetUserTodoListsAsync(string userId);

    Task<bool> ExistsAsync(int id);

    Task<bool> IsOwnerAsync(int todoListId, string userId);

    Task<bool> IsMemberAsync(int todoListId, string userId);

    Task AddAsync(TodoListEntity entity);

    Task Remove(TodoListEntity entity);

    Task<TodoListMember?> GetMemberAsync(int todoListId, string userId);

    Task AddMemberAsync(TodoListMember member);

    Task RemoveMember(TodoListMember member);

    Task ReassignTasksAsync(int todoListId, string fromUserId, string toUserId);

    Task SaveChangesAsync();
}
