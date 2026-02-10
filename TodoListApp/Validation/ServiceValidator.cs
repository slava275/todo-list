using TodoListApp.WebApi.Exceptions;

namespace TodoListApp.WebApi.Validation;

public static class ServiceValidator
{
    public static void EnsureValidId(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
    }

    public static void EnsureNotNull<T>(T? item)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(item);
    }

    public static async Task EnsureExistsAsync(int id, Func<int, Task<bool>> existsCheck, string entityName)
    {
        EnsureValidId(id);
        if (!await existsCheck(id))
        {
            throw new EntityNotFoundException($"{entityName} з ID {id} не знайдено.");
        }
    }

    public static async Task EnsureOwnerAsync(int id, string userId, Func<int, string, Task<bool>> ownerCheck)
    {
        if (!await ownerCheck(id, userId))
        {
            throw new AccessDeniedException("Тільки власник має права на цю дію.");
        }
    }
}
