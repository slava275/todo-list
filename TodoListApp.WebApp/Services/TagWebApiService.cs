using System.Text.Json;
using System.Text.Json.Serialization;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Services;

public class TagWebApiService : ITagWebApiService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public TagWebApiService(HttpClient client)
    {
        this.client = client;
    }

    public async Task AddTagToTaskAsync(int taskId, TagModel model)
    {
        var response = await this.client.PostAsJsonAsync($"tags/{taskId}", model, this.options);

        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<TagModel>> GetAllTagsAsync()
    {
        return await this.client.GetFromJsonAsync<IEnumerable<TagModel>>("tags", this.options) ?? Array.Empty<TagModel>();
    }

    public async Task<IEnumerable<TaskModel>> GetTasksByTagIdAsync(int tagId)
    {
        return await this.client.GetFromJsonAsync<IEnumerable<TaskModel>>($"tags/{tagId}/tasks", this.options) ?? Array.Empty<TaskModel>();
    }

    public async Task RemoveTagFromTaskAsync(int taskId, int tagId)
    {
        var response = await this.client.DeleteAsync($"tags/{taskId}/{tagId}");

        response.EnsureSuccessStatusCode();
    }
}
