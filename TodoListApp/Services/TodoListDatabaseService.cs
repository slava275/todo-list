using AutoMapper;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
using TodoListApp.Interfaces;
using TodoListApp.Interfaces.Repositories;
using TodoListApp.Validation;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class TodoListDatabaseService : ITodoListDatabaseService
{
    private const string EntityName = "Список";
    private readonly ITodoListRepository repository;
    private readonly IMapper mapper;

    public TodoListDatabaseService(ITodoListRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    // US02: Створення списку
    public async Task CreateAsync(TodoListModel item, string userId)
    {
        ServiceValidator.EnsureNotNull(item);

        var entity = this.mapper.Map<TodoListEntity>(item);
        await this.repository.AddAsync(entity);
        await this.repository.SaveChangesAsync();

        var membership = new TodoListMember
        {
            TodoListId = entity.Id,
            UserId = userId,
            Role = TodoListRole.Owner,
        };

        await this.repository.AddMemberAsync(membership);
        await this.repository.SaveChangesAsync();
    }

    // US03: Видалення списку (тільки Owner)
    public async Task DeleteByIdAsync(int id, string userId)
    {
        await ServiceValidator.EnsureExistsAsync(id, this.repository.ExistsAsync, EntityName);
        await ServiceValidator.EnsureOwnerAsync(id, userId, this.repository.IsOwnerAsync);

        var entity = await this.repository.GetByIdAsync(id);
        await this.repository.Remove(entity!);
        await this.repository.SaveChangesAsync();
    }

    // US01: Список усіх списків користувача
    public async Task<IEnumerable<TodoListModel>> GetAllAsync(string userId)
    {
        var entities = await this.repository.GetUserTodoListsAsync(userId);
        return this.mapper.Map<IEnumerable<TodoListModel>>(entities);
    }

    public async Task<TodoListModel?> GetByIdAsync(int id, string userId)
    {
        ServiceValidator.EnsureValidId(id);

        var entity = await this.repository.GetWithMembersAsync(id);

        if (entity == null)
        {
            throw new EntityNotFoundException("Список не знайдено");
        }

        if (!entity.TodoListMembers.Any(m => m.UserId == userId))
        {
            throw new AccessDeniedException("У вас немає доступу до цього списку.");
        }

        return this.mapper.Map<TodoListModel>(entity);
    }

    // US04: Редагування списку (тільки Owner)
    public async Task UpdateAsync(TodoListModel item, string userId)
    {
        ServiceValidator.EnsureNotNull(item);
        await ServiceValidator.EnsureExistsAsync(item.Id, this.repository.ExistsAsync, EntityName);
        await ServiceValidator.EnsureOwnerAsync(item.Id, userId, this.repository.IsOwnerAsync);

        var existingEntity = await this.repository.GetByIdAsync(item.Id);
        this.mapper.Map(item, existingEntity);
        await this.repository.SaveChangesAsync();
    }

    public async Task AddMemberAsync(int todoListId, string newMemberId, string ownerId)
    {
        await ServiceValidator.EnsureExistsAsync(todoListId, this.repository.ExistsAsync, EntityName);
        await ServiceValidator.EnsureOwnerAsync(todoListId, ownerId, this.repository.IsOwnerAsync);

        if (await this.repository.IsMemberAsync(todoListId, newMemberId))
        {
            throw new ArgumentException("Цей користувач вже є учасником списку.");
        }

        var member = new TodoListMember
        {
            TodoListId = todoListId,
            UserId = newMemberId,
            Role = TodoListRole.Viewer,
        };

        await this.repository.AddMemberAsync(member);
        await this.repository.SaveChangesAsync();
    }

    public async Task RemoveMemberAsync(int todoListId, string memberId, string ownerId)
    {
        if (memberId == ownerId)
        {
            throw new ArgumentException("Ви не можете видалити себе зі свого списку.");
        }

        await ServiceValidator.EnsureExistsAsync(todoListId, this.repository.ExistsAsync, EntityName);
        await ServiceValidator.EnsureOwnerAsync(todoListId, ownerId, this.repository.IsOwnerAsync);

        var member = await this.repository.GetMemberAsync(todoListId, memberId);
        if (member == null)
        {
            throw new EntityNotFoundException("Цей користувач не є учасником списку.");
        }

        await this.repository.RemoveMember(member);
        await this.repository.ReassignTasksAsync(todoListId, memberId, ownerId);
        await this.repository.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(int todoListId, string memberId, TodoListRole newRole, string ownerId)
    {
        await ServiceValidator.EnsureExistsAsync(todoListId, this.repository.ExistsAsync, EntityName);
        await ServiceValidator.EnsureOwnerAsync(todoListId, ownerId, this.repository.IsOwnerAsync);

        var member = await this.repository.GetMemberAsync(todoListId, memberId);
        if (member == null)
        {
            throw new EntityNotFoundException("Цей користувач не є учасником списку.");
        }

        member.Role = newRole;
        await this.repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(TodoListModel todoListModel, string userId)
    {
        ServiceValidator.EnsureNotNull(todoListModel);
        await this.DeleteByIdAsync(todoListModel.Id, userId);
    }
}
