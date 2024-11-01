using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using PharmaStockManager.Models.Identity;
using System.Threading.Tasks;

namespace PharmaStockManager.Filters
{
    

    public class EmailConfirmedFilter : IAsyncActionFilter
    {
        private readonly UserManager<AppUser> _userManager;

        public EmailConfirmedFilter(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        context.Result = new RedirectToActionResult("MailConfirm", "Account", null);
                        return;
                    }
                }
            }
            await next();
        }
    }

}
