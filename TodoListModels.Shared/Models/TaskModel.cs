namespace TodoListShared.Models.Models;

public class TaskModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int TodoListId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public bool IsCompleted => this.Status == Statuses.Completed;

    public Statuses Status { get; set; }

    public bool IsOverdue => !this.IsCompleted && this.DueDate.HasValue && this.DueDate.Value < DateTime.UtcNow;

    public ICollection<TagModel> Tags { get; set; } = [];
}
