using System.Text.Json;
using System.Text.Json.Serialization;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Services;

public class CommentsWebApiService : ICommentWebApiService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public CommentsWebApiService(HttpClient client)
    {
        this.client = client;
    }

    public async Task AddAsync(CommentModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Comment model cannot be null.");
        }

        var response = await this.client.PostAsJsonAsync("comments", model, this.options);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(errorContent);
        }
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        var response = await this.client.DeleteAsync($"comments/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(errorContent);
        }
    }

    public async Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "TaskId must be greater than zero.");
        }

        var response = await this.client.GetFromJsonAsync<IEnumerable<CommentModel>>($"comments/tasks/{taskId}", this.options);

        return response ?? Enumerable.Empty<CommentModel>();
    }

    public async Task UpdateAsync(CommentModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Comment model cannot be null.");
        }

        var response = await this.client.PutAsJsonAsync("comments", model, this.options);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();

            throw new HttpRequestException(errorContent);
        }
    }
}
