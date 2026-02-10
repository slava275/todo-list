namespace TodoListApp.WebApi.Models.Models;

public class TodoListModel
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<TaskModel> Tasks { get; set; } = [];

    public List<TodoListMemberModel> Members { get; set; } = [];
}
