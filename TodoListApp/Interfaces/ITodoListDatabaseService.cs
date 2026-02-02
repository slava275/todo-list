using TodoListApp.Data.Models;

namespace TodoListApp.Interfaces;

public interface ITodoListDatabaseService
{
    Task<IEnumerable<TodoListModel>> GetAllAsync();

    Task<TodoListModel?> GetByIdAsync(int id);

    Task CreateAsync(TodoListModel item);

    Task DeleteAsync(TodoListModel todoListModel);

    Task DeleteByIdAsync(int id);

    Task UpdateAsync(TodoListModel item);
}
