using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.Database.Data;

public static class DataSeeder
{
    public static void Seed(TodoListDbContext context)
    {
        // 1. Очищення таблиць (враховуючи каскадне видалення та нові таблиці)
        context.TodoListMembers.ExecuteDelete(); // Очищаємо зв'язки спочатку
        context.TodoLists.ExecuteDelete();
        context.Tags.ExecuteDelete();
        context.Comments.ExecuteDelete();

        // Скидаємо лічильники ID
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('TodoLists', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Tags', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Comments', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Tasks', RESEED, 0)");
        context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('TodoListMembers', RESEED, 0)");

        // Правильний ID твого користувача
        string testUserId = "aa9fc59d-8382-4ba5-b320-ed9ffae280bd";

        // 2. Створюємо спільні теги
        var tagWork = new TagEntity { Name = "Робота" };
        var tagEducation = new TagEntity { Name = "Навчання" };
        var tagUrgent = new TagEntity { Name = "Терміново" };
        var tagTravel = new TagEntity { Name = "Подорож" };

        // 3. Створення списків
        var listEpam = new TodoListEntity
        {
            Title = "Навчання в EPAM",
            Description = "Завдання по курсу .NET Fundamentals",
            Tasks = new List<TaskEntity>
            {
                new TaskEntity
                {
                    Title = "Завершити Epic 2",
                    Description = "Пройти курси",
                    Status = Statuses.Completed,
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    CreatorId = testUserId,
                    AssigneeId = "3142db48-9d6d-422d-94e8-7f7e6576dcd9",
                    Tags = new List<TagEntity> { tagEducation, tagWork },
                    Comments = new List<CommentEntity>
                    {
                        new CommentEntity { Text = "Всі юзер сторі закриті успішно!", CreatedAt = DateTime.UtcNow.AddDays(-4), UserId = testUserId },
                        new CommentEntity { Text = "Залишилось підготувати звіт.", CreatedAt = DateTime.UtcNow.AddDays(-3), UserId = testUserId },
                    },
                },
                new TaskEntity
                {
                    Title = "Реалізувати Data Seeder",
                    Description = "Зробити таску",
                    Status = Statuses.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    DueDate = DateTime.UtcNow.AddDays(2),
                    CreatorId = testUserId,
                    AssigneeId = testUserId,
                    Tags = new List<TagEntity> { tagEducation, tagUrgent },
                    Comments = new List<CommentEntity>
                    {
                        new CommentEntity { Text = "Треба не забути додати коментарі в сідер.", CreatedAt = DateTime.UtcNow, UserId = testUserId, },
                    },
                },
            },
        };

        var listPersonal = new TodoListEntity
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
                    CreatorId = testUserId,
                    AssigneeId = testUserId,
                    Tags = new List<TagEntity> { tagTravel, tagUrgent },
                    Comments = new List<CommentEntity>
                    {
                        new CommentEntity { Text = "Подивитися ціни на Intercity.", CreatedAt = DateTime.UtcNow.AddHours(-2), UserId = testUserId, },
                    },
                },
            },
        };

        context.TodoLists.AddRange(listEpam, listPersonal);
        context.SaveChanges();

        // 4. ВАЖЛИВО: Створюємо членство у списках (Epic 1)
        // Якщо цього не зробити, GetAllAsync() поверне порожній список
        var memberships = new List<TodoListMember>
        {
            new TodoListMember { TodoListId = listEpam.Id, UserId = testUserId, Role = TodoListRole.Owner },
            new TodoListMember { TodoListId = listPersonal.Id, UserId = testUserId, Role = TodoListRole.Owner },
            new TodoListMember { TodoListId = listEpam.Id, UserId = "3142db48-9d6d-422d-94e8-7f7e6576dcd9", Role = TodoListRole.Viewer },
        };

        context.TodoListMembers.AddRange(memberships);
        context.SaveChanges();
    }
}
