using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database.Data;

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

    public DbSet<TodoListMember> TodoListMembers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Конфігурація статусів завдань
        builder.Entity<TaskEntity>()
            .Property(e => e.Status)
            .HasConversion<string>();

        builder.Entity<TaskEntity>()
            .HasMany(t => t.Tags)
            .WithMany(t => t.Tasks)
            .UsingEntity(j => j.ToTable("TaskTags"));

        builder.Entity<TodoListMember>(entity =>
        {
            entity.ToTable("TodoListMembers");

            entity.HasKey(m => m.Id);

            entity.HasIndex(m => new { m.TodoListId, m.UserId }).IsUnique();

            entity.HasOne(m => m.TodoList)
                .WithMany(l => l.TodoListMembers)
                .HasForeignKey(m => m.TodoListId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(m => m.Role)
                .HasConversion<string>();
        });
    }
}
