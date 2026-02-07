using System.ComponentModel.DataAnnotations.Schema;
using TodoListShared.Models.Models;

namespace TodoList.Data.Entities;

public class TodoListMember
{
    public int Id { get; set; }

    [ForeignKey("TodoList")]
    public int TodoListId { get; set; }

    public TodoListEntity TodoList { get; set; }

    [ForeignKey("User")]
    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    public TodoListRole Role { get; set; }
}
