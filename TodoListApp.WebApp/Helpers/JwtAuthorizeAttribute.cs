using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoListApp.WebApp.Helpers;

public class JwtAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Cookies.ContainsKey("jwtToken"))
        {
            context.Result = new RedirectToActionResult("Login", "AccountApp", null);
        }
    }
}
