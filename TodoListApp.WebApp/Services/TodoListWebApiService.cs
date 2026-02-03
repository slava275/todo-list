using System.Text.Json;
using System.Text.Json.Serialization;
using TodoListApp.WebApp.Interfaces;
using TodoListShared.Models.Models;

namespace TodoListApp.WebApp.Services;

public class TodoListWebApiService : ITodoListWebApiService
{
    private readonly HttpClient httpClient;

    private readonly JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public TodoListWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task CreateAsync(TodoListModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        await this.httpClient.PostAsJsonAsync("todolists", model);
    }

    public async Task DeleteByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID must be greater than zero.", nameof(id));
        }

        var response = await this.httpClient.DeleteAsync($"todolists/{id}");

        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<TodoListModel>> GetAllAsync()
    {
        return await this.httpClient.GetFromJsonAsync<IEnumerable<TodoListModel>>("todolists", this.options)
            ?? Enumerable.Empty<TodoListModel>();
    }

    public Task<TodoListModel> GetByIdAsync(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        return this.httpClient.GetFromJsonAsync<TodoListModel>($"todolists/{id}", this.options);
    }

    public async Task UpdateAsync(TodoListModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var response = await this.httpClient.PutAsJsonAsync($"todolists/{model.Id}", model, this.options);

        response.EnsureSuccessStatusCode();
    }
}
