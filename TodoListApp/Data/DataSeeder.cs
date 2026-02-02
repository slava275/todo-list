using Microsoft.EntityFrameworkCore;
using TodoListApp.Entities;

namespace TodoListApp.Data;

public static class DataSeeder
{
    public static void Seed(TodoListDbContext context)
    {
        // 1. Очищення таблиць перед кожним запуском
        context.TodoLists.ExecuteDelete();

        // Скидаємо лічильники ID, щоб списки завжди мали Id 1 та 2
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('TodoLists', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Tasks', RESEED, 0)");

        // 2. Створення двох різних списків справ
        var lists = new List<TodoListEntity>
        {
            // ПЕРШИЙ СПИСОК: Навчання
            new TodoListEntity
            {
                Title = "Навчання в EPAM",
                Description = "Завдання по курсу .NET Fundamentals",
                Tasks = new List<TaskEntity>
                {
                    new TaskEntity
                    {
                        Title = "Завершити Epic 2",
                        Status = Statuses.Completed,
                        IsCompleted = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new TaskEntity
                    {
                        Title = "Реалізувати Data Seeder",
                        Status = Statuses.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        DueDate = DateTime.UtcNow.AddDays(2)
                    }
                }
            },
            // ДРУГИЙ СПИСОК: Особисте
            new TodoListEntity
            {
                Title = "Особисті плани",
                Description = "Побутові справи та підготовка до поїздки",
                Tasks = new List<TaskEntity>
                {
                    new TaskEntity
                    {
                        Title = "Купити квитки",
                        Description = "Перевірити потяги до Варшави",
                        Status = Statuses.NotStarted,
                        CreatedAt = DateTime.UtcNow
                    },
                    new TaskEntity
                    {
                        Title = "Прострочений таск",
                        Description = "Цей таск має бути підсвічений червоним",
                        Status = Statuses.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        DueDate = DateTime.UtcNow.AddDays(-2) // US10: Прострочено
                    }
                }
            }
        };

        context.TodoLists.AddRange(lists);
        context.SaveChanges();
    }
}
