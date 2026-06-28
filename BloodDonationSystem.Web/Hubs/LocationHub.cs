using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Hubs
{
    [Authorize]
    public class LocationHub : Hub
    {
        public async Task UpdateLocation(double latitude, double longitude, int emergencyRequestId)
        {
            var donorId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (donorId == null) return;

            await Clients.Group($"emergency_{emergencyRequestId}").SendAsync("DonorLocationUpdated", new
            {
                donorId,
                latitude,
                longitude,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task JoinEmergencyRoom(int emergencyRequestId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"emergency_{emergencyRequestId}");
        }

        public async Task LeaveEmergencyRoom(int emergencyRequestId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"emergency_{emergencyRequestId}");
        }
    }
}