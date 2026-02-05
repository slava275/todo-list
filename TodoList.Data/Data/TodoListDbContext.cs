using Microsoft.EntityFrameworkCore;
using TodoList.Data.Entities;

namespace TodoList.Data.Data;

public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists => this.Set<TodoListEntity>();

    public DbSet<TaskEntity> Tasks => this.Set<TaskEntity>();

    public DbSet<TagEntity> Tags => this.Set<TagEntity>();

    public DbSet<CommentEntity> Comments => this.Set<CommentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskEntity>()
            .Property(e => e.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TaskEntity>()
            .HasMany(t => t.Tags)
            .WithMany(t => t.Tasks)
            .UsingEntity(j => j.ToTable("TaskTags"));
    }
}
