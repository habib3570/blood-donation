using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Filters
{
    public class SpamProtectionFilter : IAsyncActionFilter
    {
        private readonly IBloodRequestService _bloodRequestService;

        public SpamProtectionFilter(IBloodRequestService bloodRequestService)
        {
            _bloodRequestService = bloodRequestService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = context.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var count = await _bloodRequestService.GetTodayRequestCountAsync(userId);
                if (count.IsSuccess && count.Data >= DonationConstants.MaxEmergencyRequestsPerDay)
                {
                    context.Result = new JsonResult(new
                    {
                        success = false,
                        message = $"Maximum {DonationConstants.MaxEmergencyRequestsPerDay} emergency requests per day allowed."
                    });
                    return;
                }
            }
            await next();
        }
    }
}