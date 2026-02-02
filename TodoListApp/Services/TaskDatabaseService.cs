using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.Data.Models;
using TodoListApp.Entities;
using TodoListApp.Interfaces;

namespace TodoListApp.Services;

public class TaskDatabaseService : ITaskDatabaseService
{
    private readonly TodoListDbContext context;
    private readonly IMapper mapper;

    public TaskDatabaseService(TodoListDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<TaskModel>> GetByListIdAsync(int todoListId)
    {
        var entities = await this.context.Tasks
            .Where(t => t.TodoListId == todoListId)
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }

    public async Task CreateAsync(TaskModel item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var entity = this.mapper.Map<TaskEntity>(item);

        if (entity.CreatedAt == default)
        {
            entity.CreatedAt = DateTime.UtcNow;
        }

        this.context.Tasks.Add(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var entity = await this.context.Tasks.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Task {id} not found");
        }

        this.context.Tasks.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskModel taskModel)
    {
        if (taskModel is null)
        {
            throw new ArgumentNullException(nameof(taskModel), "TaskModel cannot be null.");
        }

        var entity = this.mapper.Map<TaskEntity>(taskModel);
        this.context.Tasks.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        return this.mapper.Map<IEnumerable<TaskModel>>(await this.context.Tasks.ToListAsync());
    }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        return this.mapper.Map<TaskModel?>(await this.context.Tasks.FindAsync(id));
    }

    public async Task UpdateAsync(TaskModel item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        }

        var existingEntity = await this.context.Tasks.FindAsync(item.Id);

        if (existingEntity == null)
        {
            throw new KeyNotFoundException($"Task with ID {item.Id} not found.");
        }

        this.mapper.Map(item, existingEntity);

        await this.context.SaveChangesAsync();
    }
}
