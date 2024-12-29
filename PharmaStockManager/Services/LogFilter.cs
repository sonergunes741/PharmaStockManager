using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using PharmaStockManager.Models;
using PharmaStockManager.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;

namespace PharmaStockManager.Services
{
    public class LogFilter : IAsyncActionFilter
    {
        private readonly PharmaContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LogFilter(PharmaContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int? userID = null;

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var id))
                {
                    userID = id;
                }
            }

            var url = context.HttpContext.Request.Path;
            var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();

            var log = new LogClass
            {
                Url = url,
                IPAddress = ipAddress,
                CreatedAt = DateTime.UtcNow,
                UserId = userID
            };

            try
            {
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Proper logging can replace this with a logging framework (e.g., Serilog, NLog).
                Console.WriteLine($"Error while saving log: {ex.Message}");
            }

            // Ensure the action is executed
            await next();
        }

        public class LogClass
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [MaxLength(255)]  // Accommodates longer URLs
            public string Url { get; set; }

            [Required]
            [MaxLength(45)]  // Supports IPv6 addresses
            public string IPAddress { get; set; }

            [Required]
            public DateTime CreatedAt { get; set; }

            public int? UserId { get; set; }

            [ForeignKey("UserId")]
            public AppUser User { get; set; }
        }
    }
}
