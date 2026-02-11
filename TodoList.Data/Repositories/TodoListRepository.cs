using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Data;
using TodoListApp.Services.Database.Entities;
using TodoListApp.Services.Database.Interfaces.Repositories;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.Database.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly TodoListDbContext context;

    public TodoListRepository(TodoListDbContext context) => this.context = context;

    public async Task<TodoListEntity?> GetByIdAsync(int id) =>
        await this.context.TodoLists.FindAsync(id);

    public async Task<TodoListEntity?> GetWithMembersAsync(int id)
    {
        return await this.context.TodoLists
            .Include(l => l.TodoListMembers)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<IEnumerable<TodoListEntity>> GetUserTodoListsAsync(string userId)
    {
        return await this.context.TodoLists
           .Include(t => t.Tasks)
           .Where(t => this.context.TodoListMembers.Any(m => m.TodoListId == t.Id && m.UserId == userId))
           .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await this.context.TodoLists.AnyAsync(t => t.Id == id);

    public async Task<bool> IsOwnerAsync(int todoListId, string userId) =>
        await this.context.TodoListMembers.AnyAsync(m => m.TodoListId == todoListId && m.UserId == userId && m.Role == TodoListRole.Owner);

    public async Task<bool> IsMemberAsync(int todoListId, string userId) =>
        await this.context.TodoListMembers.AnyAsync(m => m.TodoListId == todoListId && m.UserId == userId);

    public async Task AddAsync(TodoListEntity entity) => await this.context.TodoLists.AddAsync(entity);

    public Task Remove(TodoListEntity entity)
    {
        this.context.TodoLists.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<TodoListMember?> GetMemberAsync(int todoListId, string userId) =>
        await this.context.TodoListMembers.FirstOrDefaultAsync(m => m.TodoListId == todoListId && m.UserId == userId);

    public async Task AddMemberAsync(TodoListMember member) => await this.context.TodoListMembers.AddAsync(member);

    public Task RemoveMember(TodoListMember member)
    {
        this.context.TodoListMembers.Remove(member);
        return Task.CompletedTask;
    }

    public async Task ReassignTasksAsync(int todoListId, string fromUserId, string toUserId)
    {
        var tasks = await this.context.Tasks
            .Where(t => t.TodoListId == todoListId && t.AssigneeId == fromUserId)
            .ToListAsync();

        foreach (var task in tasks)
        {
            task.AssigneeId = toUserId;
        }
    }

    public async Task SaveChangesAsync() => await this.context.SaveChangesAsync();
}
