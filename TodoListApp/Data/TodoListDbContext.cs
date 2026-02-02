using Microsoft.EntityFrameworkCore;
using TodoListApp.Entities;

namespace TodoListApp.Data;

public class TodoListDbContext : DbContext
{
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoListEntity> TodoLists => this.Set<TodoListEntity>();

    public DbSet<TaskEntity> Tasks => this.Set<TaskEntity>();
}
