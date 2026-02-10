using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApi.Validation;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class CommentsWebApiService : BaseWebApiService, ICommentWebApiService
{
    public CommentsWebApiService(HttpClient client, ILogger<CommentsWebApiService> logger)
        : base(client, logger)
    {
    }

    public async Task AddAsync(CommentModel model)
    {
        ServiceValidator.EnsureNotNull(model);

        var response = await this.httpClient.PostAsJsonAsync("comments", model, this.options);

        await this.HandleResponseAsync(response);
    }

    public async Task DeleteAsync(int id)
    {
        ServiceValidator.EnsureValidId(id);

        var response = await this.httpClient.DeleteAsync($"comments/{id}");

        await this.HandleResponseAsync(response);
    }

    public async Task<IEnumerable<CommentModel>> GetByTaskIdAsync(int taskId)
    {
        ServiceValidator.EnsureValidId(taskId);

        var response = await this.httpClient.GetFromJsonAsync<IEnumerable<CommentModel>>($"comments/tasks/{taskId}", this.options);

        return response ?? Enumerable.Empty<CommentModel>();
    }

    public async Task UpdateAsync(CommentModel model)
    {
        ServiceValidator.EnsureNotNull(model);

        var response = await this.httpClient.PutAsJsonAsync("comments", model, this.options);

        await this.HandleResponseAsync(response);
    }
}
