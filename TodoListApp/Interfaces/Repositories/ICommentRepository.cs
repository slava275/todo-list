using TodoListApp.Services.Database.Entities;

namespace TodoListApp.WebApi.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<CommentEntity?> GetByIdAsync(int id);

    Task<CommentEntity?> GetWithTaskAsync(int id);

    Task<IEnumerable<CommentEntity>> GetByTaskIdAsync(int taskId);

    Task AddAsync(CommentEntity entity);

    Task RemoveAsync(CommentEntity entity);

    Task<bool> ExistsAsync(int id);

    Task SaveChangesAsync();
}
