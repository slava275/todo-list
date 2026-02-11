using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Interfaces;

public interface ICommentWebApiService
{
    // US22: Отримати список коментарів (якщо знадобиться окремий виклик)
    Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId);

    // US23: Додати новий коментар до конкретного завдання
    Task AddAsync(CommentModel model);

    // US24: Видалити коментар за його ідентифікатором
    Task DeleteAsync(int id);

    // US25: Оновити текст існуючого коментаря
    Task UpdateAsync(CommentModel model);
}
