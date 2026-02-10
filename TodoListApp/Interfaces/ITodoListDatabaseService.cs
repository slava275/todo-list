using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Interfaces;

public interface ITodoListDatabaseService
{
    // US01: Отримати всі списки, до яких користувач має доступ
    Task<IEnumerable<TodoListModel>> GetAllAsync(string userId);

    // US05: Отримати конкретний список за ID (з перевіркою доступу)
    Task<TodoListModel?> GetByIdAsync(int id, string userId);

    // US02: Створити новий список (автоматично призначає творця власником)
    Task CreateAsync(TodoListModel item, string userId);

    // US03: Видалити список (тільки для власника)
    Task DeleteAsync(TodoListModel todoListModel, string userId);

    // US03: Видалити список за ID (тільки для власника)
    Task DeleteByIdAsync(int id, string userId);

    // US04: Оновити дані списку (тільки для власника)
    Task UpdateAsync(TodoListModel item, string userId);

    Task AddMemberAsync(int todoListId, string newMemberId, string ownerId);

    Task RemoveMemberAsync(int todoListId, string memberId, string ownerId);

    Task UpdateMemberRoleAsync(int todoListId, string memberId, TodoListRole newRole, string ownerId);
}
