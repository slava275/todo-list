namespace TodoListApp.WebApi.Models.Models;

public class TodoListMemberModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public TodoListRole Role { get; set; }
}
