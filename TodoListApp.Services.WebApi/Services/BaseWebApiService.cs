using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace TodoListApp.Services.WebApi.Services;

public abstract class BaseWebApiService
{
    protected readonly HttpClient httpClient;
    protected readonly JsonSerializerOptions options;
    private readonly ILogger<BaseWebApiService> logger;

    protected BaseWebApiService(HttpClient httpClient, ILogger<BaseWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };
        this.logger = logger;
    }

    protected async Task HandleResponseAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            this.logger.LogError(
                "API Error: {StatusCode}. URL: {Url}. Content: {Content}",
                response.StatusCode,
                response.RequestMessage?.RequestUri,
                errorContent);

            throw new HttpRequestException($"API Error: {response.StatusCode}. Content: {errorContent}");
        }
    }
}
