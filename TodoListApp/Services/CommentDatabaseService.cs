using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Data;
using TodoList.Data.Entities;
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

    public async Task AddAsync(CommentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = this.mapper.Map<CommentEntity>(model);

        entity.CreatedAt = DateTime.Now;

        this.context.Comments.Add(entity);

        await this.context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        var entity = await this.context.Comments.FindAsync(id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Comment with Id {id} not found.");
        }

        this.context.Comments.Remove(entity);
        await this.context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "TaskId must be greater than zero.");
        }

        var entities = await this.context.Comments
        .Where(c => c.TaskId == taskId)
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync();

        return this.mapper.Map<IEnumerable<CommentModel>>(entities);
    }

    public async Task UpdateAsync(CommentModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var entity = await this.context.Comments.FindAsync(model.Id);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Comment with Id {model.Id} not found.");
        }

        entity.Text = model.Text;
        await this.context.SaveChangesAsync();
    }
}
