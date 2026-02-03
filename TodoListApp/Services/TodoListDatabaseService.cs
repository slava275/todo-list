using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private readonly TodoListDbContext context;
    private readonly IMapper mapper;

    public TodoListDatabaseService(TodoListDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task CreateAsync(TodoListModel item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var entity = this.mapper.Map<TodoListEntity>(item);

        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TodoListModel todoListModel)
    {
        var entity = this.mapper.Map<TodoListEntity>(todoListModel);
        this.context.TodoLists.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        var entity = await this.context.TodoLists.FindAsync(id);

        if (entity is not null)
        {
            this.context.TodoLists.Remove(entity);
            await this.context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<TodoListModel>> GetAllAsync()
    {
        var entities = await this.context.TodoLists
            .Include(t => t.Tasks)
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TodoListModel>>(entities);
    }

    public async Task<TodoListModel?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        var entity = await this.context.TodoLists
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity is null ? null : this.mapper.Map<TodoListModel>(entity);
    }

    public async Task UpdateAsync(TodoListModel item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        }

        var existingEntity = await this.context.TodoLists.FindAsync(item.Id);

        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"Task with ID {item.Id} not found.");
        }

        this.mapper.Map(item, existingEntity);

        await this.context.SaveChangesAsync();
    }
}
