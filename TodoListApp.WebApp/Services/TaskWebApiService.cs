using System.Text.Json;
using System.Text.Json.Serialization;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Services;

public class TaskWebApiService : ITaskWebApiService
{
    private readonly HttpClient client;

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public TaskWebApiService(HttpClient client)
    {
        this.client = client;
    }

    public async Task CreateAsync(TaskModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.client.PostAsJsonAsync("tasks", model);

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var response = await this.client.DeleteAsync($"tasks/{id}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<TaskModel>> GetAllByListIdAsync(int listId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(listId);

        return await this.client.GetFromJsonAsync<IEnumerable<TaskModel>>($"tasks/list/{listId}", this.options)
            ?? Enumerable.Empty<TaskModel>();
    }

    public async Task<TaskModel> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        return await this.client.GetFromJsonAsync<TaskModel>($"tasks/{id}", this.options);
    }

    public async Task UpdateAsync(TaskModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.client.PutAsJsonAsync($"tasks/{model.Id}", model, this.options);

        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<TaskModel>> GetAllByUserIdAsync(Statuses? status = null, string sortBy = "name", bool isAscending = true)
    {
        var url = $"tasks/assigned?sortBy={sortBy}&isAscending={isAscending}";

        if (status.HasValue)
        {
            url += $"&status={status.Value}";
        }

        try
        {
            var response = await this.client.GetFromJsonAsync<IEnumerable<TaskModel>>(url, this.options);
            return response ?? Enumerable.Empty<TaskModel>();
        }
        catch (HttpRequestException)
        {
            return Enumerable.Empty<TaskModel>();
        }
    }

    public async Task<IEnumerable<TaskModel>> SearchAsync(string? title, DateTime? dueDate, DateTime? createdAt)
    {
        var url = "tasks/search?";
        if (!string.IsNullOrWhiteSpace(title))
        {
            url += $"title={Uri.EscapeDataString(title)}&";
        }

        if (dueDate.HasValue)
        {
            url += $"dueDate={dueDate.Value:O}&";
        }

        if (createdAt.HasValue)
        {
            url += $"createdAt={createdAt.Value:O}&";
        }

        url = url.TrimEnd('&', '?');

        try
        {
            var response = await this.client.GetFromJsonAsync<IEnumerable<TaskModel>>(url, this.options);
            return response ?? Enumerable.Empty<TaskModel>();
        }
        catch (HttpRequestException)
        {
            return Enumerable.Empty<TaskModel>();
        }
    }
}
