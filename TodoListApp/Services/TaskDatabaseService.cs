using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
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

    public async Task<IEnumerable<TaskModel>> GetByListIdAsync(int todoListId, string userId)
    {
        var hasAccess = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == todoListId && m.UserId == userId);

        if (!hasAccess)
        {
            return Enumerable.Empty<TaskModel>();
        }

        var entities = await this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => t.TodoListId == todoListId)
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }

    public async Task CreateAsync(TaskModel item, string userId)
    {
        ArgumentNullException.ThrowIfNull(item);

        var listExists = await this.context.TodoLists.AnyAsync(l => l.Id == item.TodoListId);
        if (!listExists)
        {
            throw new EntityNotFoundException($"Список з ID {item.TodoListId} не знайдено.");
        }

        var canCreate = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == item.TodoListId && m.UserId == userId &&
                           (m.Role == TodoListRole.Owner || m.Role == TodoListRole.Editor));

        if (!canCreate)
        {
            throw new AccessDeniedException("У вас немає прав для додавання завдань до цього списку.");
        }

        var entity = this.mapper.Map<TaskEntity>(item);
        entity.CreatorId = userId;
        entity.AssigneeId = userId;

        if (entity.CreatedAt == default)
        {
            entity.CreatedAt = DateTime.UtcNow;
        }

        this.context.Tasks.Add(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id, string userId)
    {
        var taskExists = await this.context.Tasks.AnyAsync(t => t.Id == id);
        if (!taskExists)
        {
            throw new EntityNotFoundException($"Завдання з ID {id} не знайдено.");
        }

        var entity = await this.context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId && m.Role == TodoListRole.Owner));

        if (entity == null)
        {
            throw new AccessDeniedException("Тільки власник списку може видаляти завдання.");
        }

        this.context.Tasks.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskModel taskModel, string userId)
    {
        ArgumentNullException.ThrowIfNull(taskModel);
        await this.DeleteByIdAsync(taskModel.Id, userId);
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync(string userId)
    {
        var entities = await this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId))
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }

    public async Task<TaskModel?> GetByIdAsync(int id, string userId)
    {
        var entity = await this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId));

        if (entity == null)
        {
            var exists = await this.context.Tasks.AnyAsync(t => t.Id == id);
            if (!exists)
            {
                throw new EntityNotFoundException($"Завдання з ID {id} не знайдено.");
            }

            throw new AccessDeniedException("У вас немає доступу до деталей цього завдання.");
        }

        return this.mapper.Map<TaskModel?>(entity);
    }

    public async Task UpdateAsync(TaskModel item, string userId)
    {
        ArgumentNullException.ThrowIfNull(item);

        var exists = await this.context.Tasks.AnyAsync(t => t.Id == item.Id);
        if (!exists)
        {
            throw new EntityNotFoundException($"Завдання з ID {item.Id} не знайдено.");
        }

        var existingEntity = await this.context.Tasks
            .FirstOrDefaultAsync(t => t.Id == item.Id &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId &&
                                               (m.Role == TodoListRole.Owner || m.Role == TodoListRole.Editor)));

        if (existingEntity == null)
        {
            throw new AccessDeniedException("У вас немає прав для редагування цього завдання.");
        }

        this.mapper.Map(item, existingEntity);
        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetAssignedTasksAsync(string userId, Statuses? status, string sortBy, bool ascending)
    {
        var query = this.context.Tasks.Where(t => t.AssigneeId == userId);

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

        return this.mapper.Map<IEnumerable<TaskModel>>(await query.ToListAsync());
    }

    public async Task ChangeStatusAsync(int id, Statuses status, string userId)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Невалідний статус.");
        }

        var taskExists = await this.context.Tasks.AnyAsync(t => t.Id == id);
        if (!taskExists)
        {
            throw new EntityNotFoundException($"Завдання з ID {id} не знайдено.");
        }

        var entity = await this.context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.AssigneeId == userId);

        if (entity == null)
        {
            throw new AccessDeniedException("Змінити статус може тільки виконавець, на якого призначене завдання.");
        }

        entity.Status = status;
        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskModel>> SerchTasksAsync(string? title, DateTime? dueDate, DateTime? createdAt, string userId)
    {
        var query = this.context.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Where(t => this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId))
            .AsQueryable();

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

        return this.mapper.Map<IEnumerable<TaskModel>>(await query.ToListAsync());
    }

    public async Task AssignTaskAsync(int taskId, string newAssigneeId, string ownerId)
    {
        var task = await this.context.Tasks
            .Include(t => t.TodoList)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
        {
            throw new EntityNotFoundException($"Завдання з ID {taskId} не знайдено.");
        }

        var isOwner = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == task.TodoListId && m.UserId == ownerId && m.Role == TodoListRole.Owner);

        if (!isOwner)
        {
            throw new AccessDeniedException("Тільки власник списку може призначати виконавців.");
        }

        var isMember = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == task.TodoListId && m.UserId == newAssigneeId);

        if (!isMember)
        {
            throw new AccessDeniedException("Цей користувач не є учасником списку.");
        }

        task.AssigneeId = newAssigneeId;
        await this.context.SaveChangesAsync();
    }
}
