using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaStockManager.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PermissionFilterAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _requiredPermissions;

        public PermissionFilterAttribute(params string[] permissions)
        {
            _requiredPermissions = permissions;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ensure user is authenticated
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Resolve dependencies
            var userManager = context.HttpContext.RequestServices.GetService(typeof(UserManager<AppUser>)) as UserManager<AppUser>;
            var dbContext = context.HttpContext.RequestServices.GetService(typeof(PharmaContext)) as PharmaContext;

            if (userManager == null || dbContext == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            // Get current authenticated user
            var user = await userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if user has active permissions
            if (!user.ActiveUser)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Retrieve user permissions
            var permissions = await dbContext.Permissions
                .FirstOrDefaultAsync(p => p.UserID == user.Id);

            if (permissions == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Validate required permissions
            bool hasAllPermissions = _requiredPermissions.All(permission =>
                permission switch
                {
                    "EditStocks" => permissions.EditStocks,
                    "StockIn" => permissions.StockIn,
                    "StockOut" => permissions.StockOut,
                    _ => false
                });

            if (!hasAllPermissions)
            {
                context.Result = new RedirectToActionResult("Error","Home",null);
                return;
            }

            // If all checks pass, proceed with the action
            await next();
        }
    }
}