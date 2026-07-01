
using AutoMapper;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Chat;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Application.Common.Interfaces;

namespace BloodDonationSystem.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatService(
            IChatRepository chatRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _chatRepository = chatRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ChatMessageDto>> SendMessageAsync(string senderId, SendMessageDto dto)
        {
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            await _chatRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<ChatMessageDto>(message);
            return Result<ChatMessageDto>.Success(result);
        }

        public async Task<Result<List<ChatMessageDto>>> GetConversationAsync(string userId1, string userId2, int page = 1)
        {
            var messages = await _chatRepository.GetConversationAsync(userId1, userId2, page);
            var dtos = _mapper.Map<List<ChatMessageDto>>(messages);
            return Result<List<ChatMessageDto>>.Success(dtos);
        }

        public async Task<Result<int>> GetUnreadCountAsync(string userId)
        {
            var count = await _chatRepository.GetUnreadCountAsync(userId);
            return Result<int>.Success(count);
        }

        public async Task<Result> MarkAsReadAsync(string senderId, string receiverId)
        {
            await _chatRepository.MarkAsReadAsync(senderId, receiverId);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success("Messages marked as read.");
        }

        public async Task<Result<List<string>>> GetChatContactsAsync(string userId)
        {
            var contacts = await _chatRepository.GetChatContactsAsync(userId);
            return Result<List<string>>.Success(contacts);
        }

        public async Task<Result<ChatMessageDto?>> GetLastMessageAsync(string userId1, string userId2)
        {
            var message = await _chatRepository.GetLastMessageAsync(userId1, userId2);
            if (message == null)
                return Result<ChatMessageDto?>.Success(null);

            var dto = _mapper.Map<ChatMessageDto>(message);
            return Result<ChatMessageDto?>.Success(dto);
        }
    }
}