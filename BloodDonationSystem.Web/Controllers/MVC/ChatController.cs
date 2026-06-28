using BloodDonationSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BloodDonationSystem.Web.Controllers.MVC
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> Index(string? userId = null)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var contacts = await _chatService.GetChatContactsAsync(currentUserId);

            ViewBag.Contacts = contacts.Data;
            ViewBag.ActiveUserId = userId;

            if (userId != null)
            {
                var messages = await _chatService.GetConversationAsync(currentUserId, userId);
                ViewBag.Messages = messages.Data;
                await _chatService.MarkAsReadAsync(userId, currentUserId);
            }

            return View();
        }
    }
}