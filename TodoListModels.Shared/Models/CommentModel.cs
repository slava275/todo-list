namespace TodoListApp.WebApi.Models.Models;

public class CommentModel
{
    public int Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int TaskId { get; set; }

    public string? UserId { get; set; }

    public string? UserName { get; set; }
}
