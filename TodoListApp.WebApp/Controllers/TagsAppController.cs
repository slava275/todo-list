using Microsoft.AspNetCore.Mvc;
using TodoListApp.WebApp.Helpers;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Controllers;

[JwtAuthorize]
public class TagsAppController : Controller
{
    private readonly ITagWebApiService tagService;

    public TagsAppController(ITagWebApiService tagService)
    {
        this.tagService = tagService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tags = await this.tagService.GetAllTagsAsync();
        return this.View(tags);
    }

    [HttpGet]
    public async Task<IActionResult> Filter(int tagId, string tagName)
    {
        this.ViewBag.TagName = tagName;
        var tasks = await this.tagService.GetTasksByTagIdAsync(tagId);
        return this.View(tasks);
    }
}
