using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApi.Interfaces;

namespace TodoListApp.WebApp.ViewComponents;

public class TodoListMembersViewComponent : ViewComponent
{
    private readonly ITodoListWebApiService todoListService;

    public TodoListMembersViewComponent(ITodoListWebApiService todoListService)
    {
        this.todoListService = todoListService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int todoListId)
    {
        var todoList = await this.todoListService.GetByIdAsync(todoListId);

        this.ViewBag.TodoListId = todoListId;
        return this.View(todoList.Members);
    }
}
