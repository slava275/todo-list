using AutoMapper;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
using TodoListApp.Interfaces;
using TodoListApp.Interfaces.Repositories;
using TodoListApp.Validation;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class CommentDatabaseService : ICommentDatabaseService
{
    private readonly ICommentRepository commentRepository;
    private readonly ITaskRepository taskRepository;
    private readonly ITodoListRepository listRepository;
    private readonly IMapper mapper;

    public CommentDatabaseService(
        ICommentRepository commentRepository,
        ITaskRepository taskRepository,
        ITodoListRepository listRepository,
        IMapper mapper)
    {
        this.commentRepository = commentRepository;
        this.taskRepository = taskRepository;
        this.listRepository = listRepository;
        this.mapper = mapper;
    }

    public async Task AddAsync(CommentModel model, string userId)
    {
        ServiceValidator.EnsureNotNull(model);

        var task = await this.taskRepository.GetByIdAsync(model.TaskId);
        if (task == null)
        {
            throw new EntityNotFoundException($"Завдання з ID {model.TaskId} не знайдено.");
        }

        var member = await this.listRepository.GetMemberAsync(task.TodoListId, userId);
        if (member == null || (member.Role != TodoListRole.Owner && member.Role != TodoListRole.Editor))
        {
            throw new AccessDeniedException("Ви не маєте прав для додавання коментарів до цього завдання.");
        }

        var entity = this.mapper.Map<CommentEntity>(model);
        entity.UserId = userId;
        entity.CreatedAt = DateTime.UtcNow;

        await this.commentRepository.AddAsync(entity);
        await this.commentRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        await ServiceValidator.EnsureExistsAsync(id, this.commentRepository.ExistsAsync, "Коментар");

        var comment = await this.commentRepository.GetWithTaskAsync(id);

        await ServiceValidator.EnsureOwnerAsync(comment!.Task.TodoListId, userId, this.listRepository.IsOwnerAsync);

        await this.commentRepository.RemoveAsync(comment);
        await this.commentRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId, string userId)
    {
        ServiceValidator.EnsureValidId(taskId);

        var task = await this.taskRepository.GetByIdAsync(taskId);
        if (task == null || !await this.listRepository.IsMemberAsync(task.TodoListId, userId))
        {
            return Enumerable.Empty<CommentModel>();
        }

        var entities = await this.commentRepository.GetByTaskIdAsync(taskId);
        return this.mapper.Map<IEnumerable<CommentModel>>(entities);
    }

    public async Task UpdateAsync(CommentModel model, string userId)
    {
        ServiceValidator.EnsureNotNull(model);
        await ServiceValidator.EnsureExistsAsync(model.Id, this.commentRepository.ExistsAsync, "Коментар");

        var entity = await this.commentRepository.GetWithTaskAsync(model.Id);

        await ServiceValidator.EnsureOwnerAsync(entity!.Task.TodoListId, userId, this.listRepository.IsOwnerAsync);

        entity.Text = model.Text;
        await this.commentRepository.SaveChangesAsync();
    }
}
