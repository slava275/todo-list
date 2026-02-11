using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoListApp.Services.Database.Entities;

public class CommentEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(300)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int TaskId { get; set; }

    [ForeignKey("TaskId")]
    public virtual TaskEntity Task { get; set; } = null!;

    public string? UserId { get; set; }
}
