using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
using TodoListApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class TagDatabaseService : ITagDatabaseService
{
    private readonly TodoListDbContext context;
    private readonly IMapper mapper;

    public TagDatabaseService(TodoListDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task AddTagToTaskAsync(int taskId, TagModel model, string userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var taskExists = await this.context.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists)
        {
            throw new EntityNotFoundException($"Завдання з ID {taskId} не знайдено.");
        }

        var task = await this.context.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId &&
                this.context.TodoListMembers.Any(m =>
                    m.TodoListId == t.TodoListId &&
                    m.UserId == userId &&
                    (m.Role == TodoListRole.Owner || m.Role == TodoListRole.Editor)));

        if (task == null)
        {
            throw new AccessDeniedException("У вас немає прав для редагування тегів у цьому завданні.");
        }

        var existingTag = await this.context.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == model.Name.ToLower());

        if (existingTag != null)
        {
            if (task.Tags.Any(t => t.Id == existingTag.Id))
            {
                return;
            }

            task.Tags.Add(existingTag);
        }
        else
        {
            var tagEntity = this.mapper.Map<TagEntity>(model);
            task.Tags.Add(tagEntity);
        }

        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TagModel>> GetAllTagsAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var tags = await this.context.Tags
            .Where(t => t.Tasks.Any(task =>
                this.context.TodoListMembers.Any(m => m.TodoListId == task.TodoListId && m.UserId == userId)))
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TagModel>>(tags);
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId, string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tagId);

        var tagExists = await this.context.Tags.AnyAsync(t => t.Id == tagId);
        if (!tagExists)
        {
            throw new EntityNotFoundException($"Тег з ID {tagId} не знайдено.");
        }

        var tasks = await this.context.Tasks
            .Include(t => t.Tags)
            .Where(t => t.Tags.Any(tag => tag.Id == tagId) &&
                this.context.TodoListMembers.Any(m => m.TodoListId == t.TodoListId && m.UserId == userId))
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TaskModel>>(tasks);
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId, string userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tagId);

        var taskExists = await this.context.Tasks.AnyAsync(t => t.Id == taskId);
        if (!taskExists)
        {
            throw new EntityNotFoundException($"Завдання з ID {taskId} не знайдено.");
        }

        var task = await this.context.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId &&
                this.context.TodoListMembers.Any(m =>
                    m.TodoListId == t.TodoListId &&
                    m.UserId == userId &&
                    (m.Role == TodoListRole.Owner || m.Role == TodoListRole.Editor)));

        if (task == null)
        {
            throw new AccessDeniedException("У вас немає прав для видалення тегів у цьому завданні.");
        }

        var tag = task.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag != null)
        {
            task.Tags.Remove(tag);
            await this.context.SaveChangesAsync();
        }
        else
        {
            throw new EntityNotFoundException($"Тег з ID {tagId} не знайдено у цьому завданні.");
        }
    }
}
