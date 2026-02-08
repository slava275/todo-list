using System.Security.Claims;

namespace TodoListApp.Extensions;

public static class ClaimsPrincipalExtension
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        var userGuidClaim = user.Claims.FirstOrDefault(c =>
            Guid.TryParse(c.Value, out _) && c.Type != "jti");

        if (userGuidClaim != null)
        {
            return userGuidClaim.Value;
        }

        return user.FindFirstValue(ClaimTypes.NameIdentifier) !;
    }
}
