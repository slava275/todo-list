using TodoListApp.Entities;

namespace TodoListApp.Data;

public static class DataSeeder
{
    public static void Seed(TodoListDbContext context)
    {
        // Перевіряємо, чи база вже не заповнена
        if (context.TodoLists.Any())
        {
            return;
        }

        var lists = new List<TodoListEntity>
        {
            new TodoListEntity
            {
                Title = "Навчання в EPAM",
                Description = "Завдання по курсу .NET Fundamentals",
                Tasks = new List<TaskEntity>
                {
                    new TaskEntity
                    {
                        Title = "Завершити Epic 1",
                        Description = "Створити базову архітектуру",
                        Status = Statuses.Completed,
                        IsCompleted = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new TaskEntity
                    {
                        Title = "Реалізувати Data Seeder",
                        Description = "Написати клас для початкових даних",
                        Status = Statuses.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        DueDate = DateTime.UtcNow.AddDays(2)
                    },
                    new TaskEntity
                    {
                        Title = "Прострочений таск для тесту US10",
                        Description = "Цей таск має бути підсвічений червоним",
                        Status = Statuses.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        DueDate = DateTime.UtcNow.AddDays(-2) // Минула дата
                    }
                }
            }
        };

        context.TodoLists.AddRange(lists);
        context.SaveChanges();
    }
}
