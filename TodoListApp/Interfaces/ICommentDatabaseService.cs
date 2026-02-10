using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.WebApi.Interfaces;

public interface ICommentDatabaseService
{
    // US22: Отримати коментарі до таски (з перевіркою доступу до списку)
    Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId, string userId);

    // US23: Додати коментар (перевірка прав Owner або Editor)
    Task AddAsync(CommentModel model, string userId);

    // US24: Видалити коментар (тільки для Owner списку)
    Task DeleteAsync(int id, string userId);

    // US25: Редагувати коментар (тільки для Owner списку)
    Task UpdateAsync(CommentModel model, string userId);
}
