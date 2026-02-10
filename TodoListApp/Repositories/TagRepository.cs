using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Data;
using TodoListApp.Services.Database.Entities;
using TodoListApp.WebApi.Interfaces.Repositories;

namespace TodoListApp.WebApi.Repositories;

public class TagRepository : ITagRepository
{
    private readonly TodoListDbContext context;

    public TagRepository(TodoListDbContext context)
    {
        this.context = context;
    }

    public async Task<TagEntity?> GetByIdAsync(int id) =>
        await this.context.Tags.FindAsync(id);

    public async Task<TagEntity?> GetByNameAsync(string name) =>
        await this.context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

    public async Task<IEnumerable<TagEntity>> GetAllForUserAsync(string userId)
    {
        return await this.context.Tags
            .Where(t => t.Tasks.Any(task =>
                this.context.TodoListMembers.Any(m => m.TodoListId == task.TodoListId && m.UserId == userId)))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await this.context.Tags.AnyAsync(t => t.Id == id);

    public async Task AddAsync(TagEntity entity) =>
        await this.context.Tags.AddAsync(entity);

    public async Task SaveChangesAsync() =>
        await this.context.SaveChangesAsync();
}
