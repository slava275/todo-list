using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoList.Data.Entities;

[Table("TodoLists")]
public class TodoListEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public ICollection<TaskEntity> Tasks { get; set; } = [];

    public ICollection<TodoListMember> TodoListMembers { get; set; } = [];
}
