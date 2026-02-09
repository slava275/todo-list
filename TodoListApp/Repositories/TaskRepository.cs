using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Interfaces.Repositories;

namespace TodoListApp.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TodoListDbContext context;

    public TaskRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<TaskEntity?> GetByIdAsync(int id) =>
        await this.context.Tasks.FindAsync(id);

    public async Task<TaskEntity?> GetWithDetailsAsync(int id)
    {
        return await this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskEntity>> GetByListIdAsync(int todoListId)
    {
        return await this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => t.TodoListId == todoListId)
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskEntity>> GetAllForUserAsync(string userId)
    {
        return await this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await this.context.Tasks.AnyAsync(t => t.Id == id);

    public IQueryable<TaskEntity> GetAssignedTasksQuery(string userId) =>
        this.context.Tasks.Where(t => t.AssigneeId == userId);

    public IQueryable<TaskEntity> SearchTasksQuery(string userId)
    {
        return this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId));
    }

    public async Task AddAsync(TaskEntity entity) =>
        await this.context.Tasks.AddAsync(entity);

    public async Task RemoveAsync(TaskEntity entity)
    {
        this.context.Tasks.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await this.context.SaveChangesAsync();
}
