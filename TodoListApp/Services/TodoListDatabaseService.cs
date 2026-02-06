using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
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

    // US02: Створення списку з автоматичним призначенням Owner
    public async Task CreateAsync(TodoListModel item, string userId)
    {
        ArgumentNullException.ThrowIfNull(item);

        var entity = this.mapper.Map<TodoListEntity>(item);
        this.context.TodoLists.Add(entity);
        await this.context.SaveChangesAsync();

        var membership = new TodoListMember
        {
            TodoListId = entity.Id,
            UserId = userId,
            Role = TodoListRole.Owner,
        };

        this.context.TodoListMembers.Add(membership);
        await this.context.SaveChangesAsync();
    }

    // US03: Видалити список може тільки Owner
    public async Task DeleteByIdAsync(int id, string userId)
    {
        var listExists = await this.context.TodoLists.AnyAsync(t => t.Id == id);
        if (!listExists)
        {
            throw new EntityNotFoundException($"Список справ з ID {id} не знайдено.");
        }

        var entity = await this.context.TodoLists
            .FirstOrDefaultAsync(t => t.Id == id &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.Id && m.UserId == userId && m.Role == TodoListRole.Owner));

        if (entity == null)
        {
            throw new AccessDeniedException("Тільки власник може видалити цей список справ.");
        }

        this.context.TodoLists.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    // US01: Бачити список усіх МОЇХ списків
    public async Task<IEnumerable<TodoListModel>> GetAllAsync(string userId)
    {
        var entities = await this.context.TodoLists
            .Include(t => t.Tasks)
            .Where(t => this.context.TodoListMembers.Any(m => m.TodoListId == t.Id && m.UserId == userId))
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TodoListModel>>(entities);
    }

    public async Task<TodoListModel?> GetByIdAsync(int id, string userId)
    {
        var entity = await this.context.TodoLists
            .Include(t => t.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id &&
                this.context.TodoListMembers.Any(m => m.TodoListId == x.Id && m.UserId == userId));

        if (entity == null)
        {
            throw new EntityNotFoundException($"Список справ з ID {id} не знайдено або у вас немає доступу.");
        }

        return this.mapper.Map<TodoListModel>(entity);
    }

    // US04: Редагувати властивості списку може тільки Owner
    public async Task UpdateAsync(TodoListModel item, string userId)
    {
        ArgumentNullException.ThrowIfNull(item);

        var listExists = await this.context.TodoLists.AnyAsync(t => t.Id == item.Id);
        if (!listExists)
        {
            throw new EntityNotFoundException($"Список справ з ID {item.Id} не знайдено.");
        }

        var existingEntity = await this.context.TodoLists
            .FirstOrDefaultAsync(t => t.Id == item.Id &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.Id && m.UserId == userId && m.Role == TodoListRole.Owner));

        if (existingEntity == null)
        {
            throw new AccessDeniedException("Тільки власник може редагувати налаштування цього списку.");
        }

        this.mapper.Map(item, existingEntity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TodoListModel todoListModel, string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentNullException.ThrowIfNull(todoListModel);
        await this.DeleteByIdAsync(todoListModel.Id, userId);
    }
}
