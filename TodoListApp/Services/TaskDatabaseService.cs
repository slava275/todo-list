using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.WebApi.Exceptions;
using TodoListApp.WebApi.Interfaces;
using TodoListApp.WebApi.Interfaces.Repositories;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Validation;

namespace TodoListApp.WebApi.Services;

public class TaskDatabaseService : ITaskDatabaseService
{
    private const string EntityName = "Завдання";
    private readonly ITaskRepository taskRepository;
    private readonly ITodoListRepository listRepository;
    private readonly IMapper mapper;

    public TaskDatabaseService(ITaskRepository taskRepository, ITodoListRepository listRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.listRepository = listRepository;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<TaskModel>> GetByListIdAsync(int todoListId, string userId)
    {
        ServiceValidator.EnsureValidId(todoListId);

        if (!await this.listRepository.IsMemberAsync(todoListId, userId))
        {
            return Enumerable.Empty<TaskModel>();
        }

        var entities = await this.taskRepository.GetByListIdAsync(todoListId);
        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }

    public async Task CreateAsync(TaskModel item, string userId)
    {
        ServiceValidator.EnsureNotNull(item);

        if (!await this.listRepository.IsMemberAsync(item.TodoListId, userId))
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

        await this.taskRepository.AddAsync(entity);
        await this.taskRepository.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id, string userId)
    {
        await ServiceValidator.EnsureExistsAsync(id, this.taskRepository.ExistsAsync, EntityName);

        var entity = await this.taskRepository.GetByIdAsync(id);

        await ServiceValidator.EnsureOwnerAsync(entity!.TodoListId, userId, this.listRepository.IsOwnerAsync);

        await this.taskRepository.RemoveAsync(entity);
        await this.taskRepository.SaveChangesAsync();
    }

    public async Task<TaskModel?> GetByIdAsync(int id, string userId)
    {
        await ServiceValidator.EnsureExistsAsync(id, this.taskRepository.ExistsAsync, EntityName);

        var entity = await this.taskRepository.GetWithDetailsAsync(id);

        if (!await this.listRepository.IsMemberAsync(entity!.TodoListId, userId))
        {
            throw new AccessDeniedException("У вас немає доступу до деталей цього завдання.");
        }

        return this.mapper.Map<TaskModel>(entity);
    }

    public async Task UpdateAsync(TaskModel item, string userId)
    {
        ServiceValidator.EnsureNotNull(item);
        await ServiceValidator.EnsureExistsAsync(item.Id, this.taskRepository.ExistsAsync, EntityName);

        var existingEntity = await this.taskRepository.GetByIdAsync(item.Id);

        var member = await this.listRepository.GetMemberAsync(existingEntity!.TodoListId, userId);
        if (member == null || (member.Role != TodoListRole.Owner && member.Role != TodoListRole.Editor))
        {
            throw new AccessDeniedException("У вас немає прав для редагування цього завдання.");
        }

        this.mapper.Map(item, existingEntity);
        await this.taskRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetAssignedTasksAsync(string userId, Statuses? status, string sortBy, bool ascending)
    {
        var query = this.taskRepository.GetAssignedTasksQuery(userId);

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

    public async Task ChangeStatusAsync(int id, Statuses status, string userId)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Невалідний статус.");
        }

        await ServiceValidator.EnsureExistsAsync(id, this.taskRepository.ExistsAsync, EntityName);

        var entity = await this.taskRepository.GetByIdAsync(id);

        if (entity!.AssigneeId != userId)
        {
            throw new AccessDeniedException("Змінити статус може тільки виконавець.");
        }

        entity.Status = status;
        await this.taskRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskModel>> SerchTasksAsync(string? title, DateTime? dueDate, DateTime? createdAt, string userId)
    {
        var query = this.taskRepository.SearchTasksQuery(userId);

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

    public async Task AssignTaskAsync(int taskId, string newAssigneeId, string ownerId)
    {
        await ServiceValidator.EnsureExistsAsync(taskId, this.taskRepository.ExistsAsync, EntityName);

        var task = await this.taskRepository.GetByIdAsync(taskId);

        await ServiceValidator.EnsureOwnerAsync(task!.TodoListId, ownerId, this.listRepository.IsOwnerAsync);

        if (!await this.listRepository.IsMemberAsync(task.TodoListId, newAssigneeId))
        {
            throw new AccessDeniedException("Цей користувач не є учасником списку.");
        }

        task.AssigneeId = newAssigneeId;
        await this.taskRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(TaskModel taskModel, string userId)
    {
        ServiceValidator.EnsureNotNull(taskModel);
        await this.DeleteByIdAsync(taskModel.Id, userId);
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync(string userId)
    {
        var entities = await this.taskRepository.GetAllForUserAsync(userId);
        return this.mapper.Map<IEnumerable<TaskModel>>(entities);
    }
}
