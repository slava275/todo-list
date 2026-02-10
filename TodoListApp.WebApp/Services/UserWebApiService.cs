using TodoListApp.WebApi.Models.Models;
using TodoListApp.WebApp.Interfaces;

namespace TodoListApp.WebApp.Services;

public class UserWebApiService : IUserWebApiService
{
    private readonly HttpClient httpClient;

    public UserWebApiService(HttpClient httpClient) => this.httpClient = httpClient;

    public async Task<IEnumerable<UserLookupModel>> SearchUsersAsync(string query)
    {
        return await this.httpClient.GetFromJsonAsync<IEnumerable<UserLookupModel>>($"Users/search?query={query}")
               ?? Enumerable.Empty<UserLookupModel>();
    }
}
