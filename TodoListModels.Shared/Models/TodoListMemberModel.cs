namespace TodoListShared.Models.Models;

public class TodoListMemberModel
{
    public int Id { get; set; }
    public int TodoListId { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public TodoListRole Role { get; set; }
}
