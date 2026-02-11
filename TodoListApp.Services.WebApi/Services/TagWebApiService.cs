using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Validation;
using TodoListApp.Services.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Services;

public class TagWebApiService : BaseWebApiService, ITagWebApiService
{
    public TagWebApiService(HttpClient client, ILogger<TagWebApiService> logger)
        : base(client, logger)
    {
    }

    public async Task AddTagToTaskAsync(int taskId, TagModel model)
    {
        ServiceValidator.EnsureValidId(taskId);
        ServiceValidator.EnsureNotNull(model);
        var response = await this.httpClient.PostAsJsonAsync($"tags/{taskId}", model, this.options);

        await this.HandleResponseAsync(response);
    }

    public async Task<IEnumerable<TagModel>> GetAllTagsAsync()
    {
        return await this.httpClient.GetFromJsonAsync<IEnumerable<TagModel>>("tags", this.options) ?? Array.Empty<TagModel>();
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId)
    {
        ServiceValidator.EnsureValidId(tagId);
        return await this.httpClient.GetFromJsonAsync<IEnumerable<TaskModel>>($"tags/{tagId}/tasks", this.options) ?? Array.Empty<TaskModel>();
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        ServiceValidator.EnsureValidId(taskId);
        ServiceValidator.EnsureValidId(tagId);
        var response = await this.httpClient.DeleteAsync($"tags/{taskId}/{tagId}");

        await this.HandleResponseAsync(response);
    }
}
