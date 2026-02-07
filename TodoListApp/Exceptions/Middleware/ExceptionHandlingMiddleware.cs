namespace TodoListApp.Exceptions.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception ex)
        {
            await this.HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var code = StatusCodes.Status500InternalServerError;
        var result = ex.Message;

        switch (ex)
        {
            case EntityNotFoundException:
                code = StatusCodes.Status404NotFound;
                this.logger.LogWarning("Entity not found: {Message}", ex.Message);
                break;
            case AccessDeniedException:
                code = StatusCodes.Status403Forbidden;
                this.logger.LogWarning("Access denied: {Message}", ex.Message);
                break;
            case ArgumentException or InvalidOperationException:
                code = StatusCodes.Status400BadRequest;
                this.logger.LogWarning("Невірний запит: {Message}", ex.Message);
                break;
            default:
                this.logger.LogError(ex, "Непередбачена помилка сервера");
                result = "Виникла внутрішня помилка сервера.";
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        await context.Response.WriteAsync(result);
    }
}
