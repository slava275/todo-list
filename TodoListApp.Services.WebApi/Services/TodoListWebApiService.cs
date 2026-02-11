using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.Validation;
using TodoListApp.Services.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Services;

public class TodoListWebApiService : BaseWebApiService, ITodoListWebApiService
{
    public TodoListWebApiService(HttpClient httpClient, ILogger<TodoListWebApiService> logger)
        : base(httpClient, logger)
    {
    }

    public async Task CreateAsync(TodoListModel model)
    {
        ServiceValidator.EnsureNotNull(model);

        var response = await this.httpClient.PostAsJsonAsync("todolists", model, this.options);
        await this.HandleResponseAsync(response);
    }

    public async Task DeleteByIdAsync(int id)
    {
        ServiceValidator.EnsureValidId(id);

        var response = await this.httpClient.DeleteAsync($"todolists/{id}");
        await this.HandleResponseAsync(response);
    }

    public async Task<IEnumerable<TodoListModel>> GetAllAsync()
    {
        return await this.httpClient.GetFromJsonAsync<IEnumerable<TodoListModel>>("todolists", this.options)
               ?? Enumerable.Empty<TodoListModel>();
    }

    public async Task<TodoListModel> GetByIdAsync(int id)
    {
        ServiceValidator.EnsureValidId(id);

        var result = await this.httpClient.GetFromJsonAsync<TodoListModel>($"todolists/{id}", this.options);
        return result ?? throw new HttpRequestException("Не вдалося отримати дані списку.");
    }

    public async Task UpdateAsync(TodoListModel model)
    {
        ServiceValidator.EnsureNotNull(model);

        var response = await this.httpClient.PutAsJsonAsync($"todolists/{model.Id}", model, this.options);
        await this.HandleResponseAsync(response);
    }

    public async Task AddMemberAsync(int todoListId, string userId)
    {
        ServiceValidator.EnsureValidId(todoListId);

        var response = await this.httpClient.PostAsync($"todolists/{todoListId}/members?userId={userId}", null);
        await this.HandleResponseAsync(response);
    }

    public async Task RemoveMemberAsync(int todoListId, string memberId)
    {
        ServiceValidator.EnsureValidId(todoListId);

        var response = await this.httpClient.DeleteAsync($"todolists/{todoListId}/members/{memberId}");
        await this.HandleResponseAsync(response);
    }

    public async Task UpdateMemberRoleAsync(int todoListId, string memberId, TodoListRole newRole)
    {
        ServiceValidator.EnsureValidId(todoListId);

        var response = await this.httpClient.PatchAsync($"todolists/{todoListId}/members/{memberId}/role?newRole={newRole}", null);
        await this.HandleResponseAsync(response);
    }
}
