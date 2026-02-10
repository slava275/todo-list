using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Services.Database.Entities;

public class TagEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    public ICollection<TaskEntity> Tasks { get; set; } = [];
}
