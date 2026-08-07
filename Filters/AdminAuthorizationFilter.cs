using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace San_Pham_Do_An1.Filters
{
    public class AdminAuthorizationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var area = context.RouteData.Values["area"]?.ToString();
            var controller = context.RouteData.Values["controller"]?.ToString();

            if (area == "Admin" && controller != "Accounts")
            {
                var roleId = context.HttpContext.Session.GetString("RoleId");
                if (roleId != "1")
                {
                    context.Result = new RedirectToActionResult("Login", "Accounts", new { area = "Admin" });
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
