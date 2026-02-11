using System.Net.Http.Json;
using TodoListApp.Services.WebApi.Interfaces;
using TodoListApp.WebApi.Models.Models;

namespace TodoListApp.Services.WebApi.Services;

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
