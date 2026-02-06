namespace TodoListApp.Exceptions;

public class AccessDeniedException : Exception
{
    private const string DefaultMessage = "У вас немає прав для виконання цієї операції.";

    public AccessDeniedException()
        : base(DefaultMessage)
    {
    }

    public AccessDeniedException(string message)
        : base(message)
    {
    }

    public AccessDeniedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
