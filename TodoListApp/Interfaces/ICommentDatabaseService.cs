using TodoListShared.Models.Models;

namespace TodoListApp.Interfaces;

public interface ICommentDatabaseService
{
    Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId);

    Task AddAsync(CommentModel model);

    Task DeleteAsync(int id);

    Task UpdateAsync(CommentModel model);
}
