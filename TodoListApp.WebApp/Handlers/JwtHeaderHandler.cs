namespace TodoListApp.WebApp.Handlers;

public class JwtHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor contextAccessor;

    public JwtHeaderHandler(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = this.contextAccessor.HttpContext?.Request.Cookies["jwtToken"];
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);

    }
}
