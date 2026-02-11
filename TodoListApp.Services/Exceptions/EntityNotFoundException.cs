namespace TodoListApp.Services.Exceptions;

public class EntityNotFoundException : Exception
{
    private const string DefaultMessage = "Запитуваний ресурс не знайдено.";

    public EntityNotFoundException()
        : base(DefaultMessage)
    {
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
