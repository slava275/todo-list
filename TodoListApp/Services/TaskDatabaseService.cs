using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Interfaces;
using TodoListShared.Models;
using TodoListShared.Models.Models;

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
            .Include(t => t.Tags)
            .Include(t => t.Comments)
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
        return this.mapper.Map<IEnumerable<TaskModel>>(await this.context.Tasks.Include(t => t.Tags)
                        .Include(t => t.Comments)
                        .ToListAsync());
    }

    public async Task<TaskModel?> GetByIdAsync(int id)
    {
        return this.mapper.Map<TaskModel?>(await this.context.Tasks.Include(t => t.Tags)
                        .Include(t => t.Comments)
                        .FirstOrDefaultAsync(t => t.Id == id));
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

    public async Task<IEnumerable<TaskModel>> GetAssignedTasksAsync(int userId, Statuses? status, string sortBy, bool ascending)
    {
        var query = this.context.Tasks.Where(t => t.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }
        else
        {
            query = query.Where(t => t.Status != Statuses.Completed && t.Status != Statuses.Сancelled);
        }

        query = sortBy?.ToLower(CultureInfo.InvariantCulture) switch
        {
            "name" => ascending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
            "duedate" => ascending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
            _ => query.OrderBy(t => t.CreatedAt)
        };

        var entities = await query.ToListAsync();
        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }

    public async Task ChangeStatusAsync(int id, Statuses status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Invalid status value.", nameof(status));
        }

        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "ID must be a positive integer.");
        }

        var entity = await this.context.Tasks.FindAsync(id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Task with ID {id} not found.");
        }

        entity.Status = status;

        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskModel>> SerchTasksAsync(string? title, DateTime? dueDate, DateTime? createdAt)
    {
        var query = this.context.Tasks.Include(t => t.Tags).Include(t => t.Comments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(t => t.Title.Contains(title));
        }

        if (dueDate.HasValue)
        {
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);
        }

        if (createdAt.HasValue)
        {
            query = query.Where(t => t.CreatedAt.Date == createdAt.Value.Date);
        }

        var entities = await query.ToListAsync();

        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }
}
