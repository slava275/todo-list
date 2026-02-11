using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Validation;
using TodoListApp.Services.WebApi.Interfaces;
using TodoListApp.WebApi.Models;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Services;

public class TaskWebApiService : BaseWebApiService, ITaskWebApiService
{
    public TaskWebApiService(HttpClient client, ILogger<TaskWebApiService> logger)
        : base(client, logger)
    {
    }

    public async Task CreateAsync(TaskModel model)
    {
        ServiceValidator.EnsureNotNull(model);

        var response = await this.httpClient.PostAsJsonAsync("tasks", model);

        await this.HandleResponseAsync(response);
    }

    public async Task DeleteByIdAsync(int id)
    {
        ServiceValidator.EnsureValidId(id);

        var response = await this.httpClient.DeleteAsync($"tasks/{id}");

        await this.HandleResponseAsync(response);
    }

    public async Task<IEnumerable<TaskModel>> GetAllByListIdAsync(int listId)
    {
        ServiceValidator.EnsureValidId(listId);

        return await this.httpClient.GetFromJsonAsync<IEnumerable<TaskModel>>($"tasks/list/{listId}", this.options)
            ?? Enumerable.Empty<TaskModel>();
    }

    public async Task<TaskModel> GetByIdAsync(int id)
    {
        ServiceValidator.EnsureValidId(id);

        return await this.httpClient.GetFromJsonAsync<TaskModel>($"tasks/{id}", this.options);
    }

    public async Task UpdateAsync(TaskModel model)
    {
        ServiceValidator.EnsureNotNull(model);

        var response = await this.httpClient.PutAsJsonAsync($"tasks/{model.Id}", model, this.options);

        await this.HandleResponseAsync(response);
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
            var response = await this.httpClient.GetFromJsonAsync<IEnumerable<TaskModel>>(url, this.options);
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
            var response = await this.httpClient.GetFromJsonAsync<IEnumerable<TaskModel>>(url, this.options);
            return response ?? Enumerable.Empty<TaskModel>();
        }
        catch (HttpRequestException)
        {
            return Enumerable.Empty<TaskModel>();
        }
    }

    public async Task AssignTaskAsync(int taskId, string userId)
    {
        ServiceValidator.EnsureValidId(taskId);
        var response = await this.httpClient.PostAsync($"tasks/{taskId}/assign?userId={userId}", null);

        await this.HandleResponseAsync(response);
    }
}
