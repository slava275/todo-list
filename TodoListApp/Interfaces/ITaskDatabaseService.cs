using TodoListShared.Models;
using TodoListShared.Models.Models;

namespace TodoListApp.Interfaces;

public interface ITaskDatabaseService
{
    // US05: Отримати таски конкретного списку (з перевіркою доступу до списку)
    Task<IEnumerable<TaskModel>> GetByListIdAsync(int todoListId, string userId);

    // US11-13: Отримати таски, призначені безпосередньо юзеру
    Task<IEnumerable<TaskModel>> GetAssignedTasksAsync(string userId, Statuses? status, string sortBy, bool ascending);

    // US14: Змінити статус (тільки якщо юзер — Assignee)
    Task ChangeStatusAsync(int id, Statuses status, string userId);

    // Отримати всі таски з усіх доступних юзеру списків
    Task<IEnumerable<TaskModel>> GetAllAsync(string userId);

    // US06: Деталі конкретної таски (з перевіркою доступу до списку)
    Task<TaskModel?> GetByIdAsync(int id, string userId);

    // US07: Створити таску (з автоматичним заповненням CreatorId/AssigneeId)
    Task CreateAsync(TaskModel item, string userId);

    // US08: Видалити таску (перевірка прав Owner)
    Task DeleteAsync(TaskModel taskModel, string userId);

    // US08: Видалити таску за ID (перевірка прав Owner)
    Task DeleteByIdAsync(int id, string userId);

    // US09: Редагувати таску (перевірка прав Owner/Editor)
    Task UpdateAsync(TaskModel item, string userId);

    // US15: Пошук по тасках у доступних списках
    Task<IEnumerable<TaskModel>> SerchTasksAsync(string? title, DateTime? dueDate, DateTime? createdAt, string userId);

    Task AssignTaskAsync(int taskId, string newAssigneeId, string ownerId);
}
