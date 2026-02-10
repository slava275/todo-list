using Microsoft.AspNetCore.Mvc;

namespace TodoListApp.WebApp.Controllers;

public abstract class BaseController : Controller
{
    protected void HandleException(Exception ex, string? modelErrorKey = null)
    {
        var message = ex.Message;

        // Якщо є ключ для моделі, додаємо в ModelState (для відображення біля полів)
        if (modelErrorKey != null)
        {
            this.ModelState.AddModelError(modelErrorKey, message);
        }
        else
        {
            this.TempData["ErrorMessage"] = message;
        }
    }
}
