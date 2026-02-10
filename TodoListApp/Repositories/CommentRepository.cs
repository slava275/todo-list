using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Data;
using TodoListApp.Services.Database.Entities;
using TodoListApp.WebApi.Interfaces.Repositories;

namespace TodoListApp.WebApi.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly TodoListDbContext context;

    public CommentRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<CommentEntity?> GetByIdAsync(int id) =>
        await this.context.Comments.FindAsync(id);

    public async Task<CommentEntity?> GetWithTaskAsync(int id)
    {
        return await this.context.Comments
            .Include(c => c.Task)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<CommentEntity>> GetByTaskIdAsync(int taskId)
    {
        return await this.context.Comments
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(CommentEntity entity) =>
        await this.context.Comments.AddAsync(entity);

    public async Task RemoveAsync(CommentEntity entity)
    {
        this.context.Comments.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await this.context.SaveChangesAsync();

    public async Task<bool> ExistsAsync(int id)
    {
        return await this.context.Comments.AnyAsync(t => t.Id == id);
    }
}
