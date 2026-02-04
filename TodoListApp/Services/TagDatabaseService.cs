using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
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

    public async Task AddTagToTaskAsync(int taskId, TagModel model, int userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);
        ArgumentOutOfRangeException.ThrowIfNegative(userId); // or zero

        var task = await this.context.Tasks
            .Include(t => t.Tags)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {taskId} not found for user ID {userId}.");
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

    public async Task<IEnumerable<TagModel>> GetAllTagsAsync(int userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(userId);

        var tags = await this.context.Tags.Where(t => t.Tasks.Any(task => task.UserId == userId)).ToListAsync();

        return this.mapper.Map<IEnumerable<TagModel>>(tags);
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId, int userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tagId);

        var tasks = await this.context.Tasks
            .Include(t => t.Tags)
            .Where(t => t.UserId == userId && t.Tags.Any(tag => tag.Id == tagId))
            .ToListAsync();

        return this.mapper.Map<IEnumerable<TaskModel>>(tasks);
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId, int userId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(taskId);
        ArgumentOutOfRangeException.ThrowIfNegative(userId);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tagId);

        var task = await this.context.Tasks.Include(t => t.Tags).FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {taskId} not found for user ID {userId}.");
        }

        var tag = task.Tags.FirstOrDefault(t => t.Id == tagId);

        if (tag == null)
        {
            throw new InvalidOperationException($"Tag with ID {tagId} not found for user ID {userId}.");
        }

        task.Tags.Remove(tag);

        await this.context.SaveChangesAsync();
    }
}
