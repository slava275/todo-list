using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoList.Data.Entities;

namespace TodoList.Data.Data;

public class TodoListDbContext : IdentityDbContext<ApplicationUser>
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists => this.Set<TodoListEntity>();

    public DbSet<TaskEntity> Tasks => this.Set<TaskEntity>();

    public DbSet<TagEntity> Tags => this.Set<TagEntity>();

    public DbSet<CommentEntity> Comments => this.Set<CommentEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<TaskEntity>()
            .Property(e => e.Status)
            .HasConversion<string>();

        builder.Entity<TaskEntity>()
            .HasMany(t => t.Tags)
            .WithMany(t => t.Tasks)
            .UsingEntity(j => j.ToTable("TaskTags"));
    }
}
