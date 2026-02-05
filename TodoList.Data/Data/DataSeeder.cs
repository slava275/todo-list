using Microsoft.EntityFrameworkCore;
using TodoList.Data.Entities;
using TodoListShared.Models;

namespace TodoList.Data.Data;

public static class DataSeeder
{
    public static void Seed(TodoListDbContext context)
    {
        // 1. Очищення таблиць (враховуючи каскадне видалення)
        context.TodoLists.ExecuteDelete();
        context.Tags.ExecuteDelete();
        context.Comments.ExecuteDelete(); // Очищаємо також таблицю коментарів

        // Скидаємо лічильники ID
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('TodoLists', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Tags', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Comments', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Tasks', RESEED, 0)");


        // 2. Створюємо спільні теги
        var tagWork = new TagEntity { Name = "Робота" };
        var tagEducation = new TagEntity { Name = "Навчання" };
        var tagUrgent = new TagEntity { Name = "Терміново" };
        var tagTravel = new TagEntity { Name = "Подорож" };

        // 3. Створення списків із завданнями, тегами та коментарями
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
                        Title = "Завершити Epic 2",
                        Status = Statuses.Completed,
                        IsCompleted = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UserId = 0,
                        Tags = new List<TagEntity> { tagEducation, tagWork },
                        Comments = new List<CommentEntity> // Додаємо коментарі (US22)
                        {
                            new CommentEntity { Text = "Всі юзер сторі закриті успішно!", CreatedAt = DateTime.UtcNow.AddDays(-4)},
                            new CommentEntity { Text = "Залишилось підготувати звіт.", CreatedAt = DateTime.UtcNow.AddDays(-3) }
                        }
                    },
                    new TaskEntity
                    {
                        Title = "Реалізувати Data Seeder",
                        Status = Statuses.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        DueDate = DateTime.UtcNow.AddDays(2),
                        UserId = 0,
                        Tags = new List<TagEntity> { tagEducation, tagUrgent },
                        Comments = new List<CommentEntity>
                        {
                            new CommentEntity { Text = "Треба не забути додати коментарі в сідер.", CreatedAt = DateTime.UtcNow }
                        }
                    }
                }
            },
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
                        CreatedAt = DateTime.UtcNow,
                        UserId = 0,
                        Tags = new List<TagEntity> { tagTravel, tagUrgent },
                        Comments = new List<CommentEntity>
                        {
                            new CommentEntity { Text = "Подивитися ціни на Intercity.", CreatedAt = DateTime.UtcNow.AddHours(-2) }
                        }
                    },
                    new TaskEntity
                    {
                        Title = "Прострочений таск",
                        Description = "Цей таск має бути підсвічений червоним",
                        Status = Statuses.InProgress,
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        DueDate = DateTime.UtcNow.AddDays(-2),
                        UserId = 0,
                        Tags = new List<TagEntity> { tagUrgent }
                        // Коментарів немає - перевіримо пустий стан
                    }
                }
            }
        };

        context.TodoLists.AddRange(lists);
        context.SaveChanges();
    }
}
