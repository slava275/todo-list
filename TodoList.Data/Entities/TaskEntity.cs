using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TodoListShared.Models;

namespace TodoList.Data.Entities;

[Table("Tasks")]
public class TaskEntity
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string AssigneeId { get; set; }

    [Required]
    public string CreatorId { get; set; }

    [ForeignKey("TodoList")]
    public int TodoListId { get; set; }

    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "datetime2")]
    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public Statuses Status { get; set; } = Statuses.NotStarted;

    public ICollection<TagEntity> Tags { get; set; } = [];

    public virtual ICollection<CommentEntity> Comments { get; set; } = [];

    public TodoListEntity? TodoList { get; set; }
}
