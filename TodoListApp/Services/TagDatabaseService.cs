using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Entities;
using TodoListApp.Exceptions;
using TodoListApp.Interfaces;
using TodoListApp.Interfaces.Repositories;
using TodoListApp.Validation;
using TodoListShared.Models.Models;

namespace TodoListApp.Services;

public class TagDatabaseService : ITagDatabaseService
{
    private readonly ITagRepository tagRepository;
    private readonly ITaskRepository taskRepository;
    private readonly ITodoListRepository listRepository;
    private readonly IMapper mapper;

    public TagDatabaseService(
        ITagRepository tagRepository,
        ITaskRepository taskRepository,
        ITodoListRepository listRepository,
        IMapper mapper)
    {
        this.tagRepository = tagRepository;
        this.taskRepository = taskRepository;
        this.listRepository = listRepository;
        this.mapper = mapper;
    }

    public async Task AddTagToTaskAsync(int taskId, TagModel model, string userId)
    {
        ServiceValidator.EnsureValidId(taskId);
        ServiceValidator.EnsureNotNull(model);

        var task = await this.taskRepository.GetWithDetailsAsync(taskId);
        if (task == null)
        {
            throw new EntityNotFoundException($"Завдання з ID {taskId} не знайдено.");
        }

        var member = await this.listRepository.GetMemberAsync(task.TodoListId, userId);
        if (member == null || (member.Role != TodoListRole.Owner && member.Role != TodoListRole.Editor))
        {
            throw new AccessDeniedException("У вас немає прав для редагування тегів у цьому завданні.");
        }

        var existingTag = await this.tagRepository.GetByNameAsync(model.Name);

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

        await this.taskRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<TagModel>> GetAllTagsAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);

        var tags = await this.tagRepository.GetAllForUserAsync(userId);
        return this.mapper.Map<IEnumerable<TagModel>>(tags);
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId, string userId)
    {
        await ServiceValidator.EnsureExistsAsync(tagId, this.tagRepository.ExistsAsync, "Тег");

        var query = this.taskRepository.SearchTasksQuery(userId)
            .Where(t => t.Tags.Any(tag => tag.Id == tagId));

        var tasks = await query.ToListAsync();
        return this.mapper.Map<IEnumerable<TaskModel>>(tasks);
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId, string userId)
    {
        ServiceValidator.EnsureValidId(taskId);
        ServiceValidator.EnsureValidId(tagId);

        var task = await this.taskRepository.GetWithDetailsAsync(taskId);
        ServiceValidator.EnsureNotNull(task);

        var member = await this.listRepository.GetMemberAsync(task!.TodoListId, userId);
        if (member == null || (member.Role != TodoListRole.Owner && member.Role != TodoListRole.Editor))
        {
            throw new AccessDeniedException("У вас немає прав для видалення тегів у цьому завданні.");
        }

        var tag = task.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag != null)
        {
            task.Tags.Remove(tag);
            await this.taskRepository.SaveChangesAsync();
        }
        else
        {
            throw new EntityNotFoundException($"Тег з ID {tagId} не знайдено у цьому завданні.");
        }
    }
}
