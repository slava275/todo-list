using TodoListApp.Entities;

namespace TodoListApp.Data.Models;

public class TaskModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public Statuses Status { get; set; }
}
