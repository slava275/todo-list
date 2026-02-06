using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class CommentDatabaseService : ICommentDatabaseService
{
    private readonly TodoListDbContext context;
    private readonly IMapper mapper;

    public CommentDatabaseService(TodoListDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task AddAsync(CommentModel model, string userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var task = await this.context.Tasks.AnyAsync(t => t.Id == model.TaskId);
        if (!task)
        {
            throw new EntityNotFoundException($"Завдання з ID {model.TaskId} не знайдено.");
        }

        var canComment = await this.context.Tasks
            .AnyAsync(t => t.Id == model.TaskId &&
                this.context.TodoListMembers.Any(m =>
                    m.TodoListId == t.TodoListId &&
                    m.UserId == userId &&
                    (m.Role == TodoListRole.Owner || m.Role == TodoListRole.Editor)));

        if (!canComment)
        {
            throw new AccessDeniedException("Ви не маєте прав для додавання коментарів до цього завдання.");
        }

        var entity = this.mapper.Map<CommentEntity>(model);
        entity.UserId = userId;
        entity.CreatedAt = DateTime.UtcNow;

        this.context.Comments.Add(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var comment = await this.context.Comments
            .Include(c => c.Task)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment == null)
        {
            throw new EntityNotFoundException($"Коментар з ID {id} не знайдено.");
        }

        var isOwner = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == comment.Task.TodoListId &&
                           m.UserId == userId &&
                           m.Role == TodoListRole.Owner);

        if (!isOwner)
        {
            throw new AccessDeniedException("Тільки власник списку може видаляти коментарі.");
        }

        this.context.Comments.Remove(comment);
        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId, string userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);

        var hasAccess = await this.context.Tasks
            .AnyAsync(t => t.Id == taskId &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId));

        if (!hasAccess)
        {
            return Enumerable.Empty<CommentModel>();
        }

        var entities = await this.context.Comments
            .Where(c => c.TaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return this.mapper.Map<IEnumerable<CommentModel>>(entities);
    }

    public async Task UpdateAsync(CommentModel model, string userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = await this.context.Comments
            .Include(c => c.Task)
            .FirstOrDefaultAsync(c => c.Id == model.Id);

        if (entity == null)
        {
            throw new EntityNotFoundException($"Коментар з ID {model.Id} не знайдено.");
        }

        var isOwner = await this.context.TodoListMembers
            .AnyAsync(m => m.TodoListId == entity.Task.TodoListId &&
                           m.UserId == userId &&
                           m.Role == TodoListRole.Owner);

        if (!isOwner)
        {
            throw new AccessDeniedException("Тільки власник списку може редагувати коментарі.");
        }

        entity.Text = model.Text;
        await this.context.SaveChangesAsync();
    }
}
