using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Interfaces.Repositories;

public interface ITagRepository
{
    Task<TagEntity?> GetByIdAsync(int id);

    Task<TagEntity?> GetByNameAsync(string name);

    Task<IEnumerable<TagEntity>> GetAllForUserAsync(string userId);

    Task<bool> ExistsAsync(int id);

    Task AddAsync(TagEntity entity);

    Task SaveChangesAsync();
}
