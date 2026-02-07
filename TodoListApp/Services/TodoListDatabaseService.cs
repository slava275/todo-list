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
            .Include(l => l.TodoListMembers)
                .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (entity == null)
        {
            throw new EntityNotFoundException($"Список {id} не знайдено.");
        }

        var isMember = entity.TodoListMembers.Any(m => m.UserId == userId);
        if (!isMember)
        {
            throw new AccessDeniedException("У вас немає доступу до цього списку.");
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

    public async Task AddMemberAsync(int todoListId, string newMemberId, string ownerId)
    {
        var todoList = await this.context.TodoLists.FindAsync(todoListId);
        if (todoList == null)
        {
            throw new EntityNotFoundException($"Список {todoListId} не знайдено.");
        }

        var isOwner = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == todoListId && m.UserId == ownerId && m.Role == TodoListRole.Owner);

        if (!isOwner)
        {
            throw new AccessDeniedException("Тільки власник може додавати нових учасників.");
        }

        var alreadyMember = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == todoListId && m.UserId == newMemberId);

        if (alreadyMember)
        {
            throw new ArgumentException("Цей користувач вже є учасником списку.");
        }

        var member = new TodoListMember
        {
            TodoListId = todoListId,
            UserId = newMemberId,
            Role = TodoListRole.Viewer,
        };

        this.context.TodoListMembers.Add(member);
        await this.context.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int todoListId, string memberId, string ownerId)
    {
        if (memberId == ownerId)
        {
            throw new ArgumentException("Ви не можете видалити себе зі свого списку. Видаліть увесь список, якщо він більше не потрібен.");
        }

        var isOwner = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == todoListId && m.UserId == ownerId && m.Role == TodoListRole.Owner);

        if (!isOwner)
        {
            throw new AccessDeniedException("Тільки власник може видаляти учасників.");
        }

        var member = await this.context.TodoListMembers
            .FirstOrDefaultAsync(m => m.TodoListId == todoListId && m.UserId == memberId);

        if (member == null)
        {
            throw new EntityNotFoundException("Цей користувач не є учасником списку.");
        }

        this.context.TodoListMembers.Remove(member);

        var assignedTasks = await this.context.Tasks
            .Where(t => t.TodoListId == todoListId && t.AssigneeId == memberId)
            .ToListAsync();

        foreach (var task in assignedTasks)
        {
            task.AssigneeId = ownerId;
        }

        await this.context.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(int todoListId, string memberId, TodoListRole newRole, string ownerId)
    {
        var isOwner = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == todoListId && m.UserId == ownerId && m.Role == TodoListRole.Owner);

        if (!isOwner)
        {
            throw new AccessDeniedException("Тільки власник може змінювати ролі учасників.");
        }

        var member = await this.context.TodoListMembers
            .FirstOrDefaultAsync(m => m.TodoListId == todoListId && m.UserId == memberId);

        if (member == null)
        {
            throw new EntityNotFoundException("Користувача не знайдено серед учасників.");
        }

        member.Role = newRole;
        await this.context.SaveChangesAsync();
    }
}
