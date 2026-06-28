using AutoMapper;
using BloodDonationSystem.Application.DTOs;
using BloodDonationSystem.Application.DTOs.Auth;
using BloodDonationSystem.Application.DTOs.BloodBank;
using BloodDonationSystem.Application.DTOs.BloodRequest;
using BloodDonationSystem.Application.DTOs.Certificate;
using BloodDonationSystem.Application.DTOs.Chat;
using BloodDonationSystem.Application.DTOs.Donation;
using BloodDonationSystem.Application.DTOs.Donor;
using BloodDonationSystem.Application.DTOs.Emergency;
using BloodDonationSystem.Application.DTOs.Hospital;
using BloodDonationSystem.Application.DTOs.Location;
using BloodDonationSystem.Application.DTOs.Notification;
using BloodDonationSystem.Application.DTOs.Rating;
using BloodDonationSystem.Application.DTOs.User;
using BloodDonationSystem.Domain.Entities;
using BloodDonationSystem.Domain.Enums;

namespace BloodDonationSystem.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<ApplicationUser, UserProfileDto>()
                .ForMember(d => d.Age, o => o.MapFrom(s => DateTime.Now.Year - s.DateOfBirth.Year))
                .ForMember(d => d.BloodGroupDisplay, o => o.MapFrom(s => GetBloodGroupDisplay(s.BloodGroup)))
                .ForMember(d => d.DonorInfo, o => o.MapFrom(s => s.DonorProfile));

            CreateMap<DonorProfile, DonorInfoDto>()
                .ForMember(d => d.Level, o => o.MapFrom(s => s.Level.ToString()));

            // Donor
            CreateMap<DonorProfile, DonorDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.ProfileImageUrl, o => o.MapFrom(s => s.User.ProfileImageUrl))
                .ForMember(d => d.BloodGroup, o => o.MapFrom(s => s.User.BloodGroup))
                .ForMember(d => d.BloodGroupDisplay, o => o.MapFrom(s => GetBloodGroupDisplay(s.User.BloodGroup)))
                .ForMember(d => d.Gender, o => o.MapFrom(s => s.User.Gender))
                .ForMember(d => d.District, o => o.MapFrom(s => s.User.District))
                .ForMember(d => d.Upazila, o => o.MapFrom(s => s.User.Upazila))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.User.PhoneNumber))
                .ForMember(d => d.LastSeenAt, o => o.MapFrom(s => s.User.LastSeenAt))
                .ForMember(d => d.LevelDisplay, o => o.MapFrom(s => s.Level.ToString()))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId));

            // BloodRequest
            CreateMap<BloodRequest, BloodRequestDto>()
                .ForMember(d => d.RequesterName, o => o.MapFrom(s => s.Requester.FullName))
                .ForMember(d => d.DonorName, o => o.MapFrom(s => s.Donor != null ? s.Donor.FullName : null))
                .ForMember(d => d.BloodGroupDisplay, o => o.MapFrom(s => GetBloodGroupDisplay(s.BloodGroup)))
                .ForMember(d => d.StatusDisplay, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<CreateBloodRequestDto, BloodRequest>();

            // Emergency
            CreateMap<EmergencyRequest, EmergencyRequestDto>()
                .ForMember(d => d.RequesterName, o => o.MapFrom(s => s.Requester.FullName))
                .ForMember(d => d.BloodGroupDisplay, o => o.MapFrom(s => GetBloodGroupDisplay(s.BloodGroup)));

            CreateMap<CreateEmergencyRequestDto, EmergencyRequest>();

            // Donation
            CreateMap<Donation, DonationDto>()
                .ForMember(d => d.DonorName, o => o.MapFrom(s => s.DonorProfile.User.FullName));

            CreateMap<Donation, DonationHistoryDto>()
                .ForMember(d => d.CertificateNumber, o => o.MapFrom(s => s.Certificate != null ? s.Certificate.CertificateNumber : null));

            // Rating
            CreateMap<DonorRating, RatingDto>()
                .ForMember(d => d.RaterName, o => o.MapFrom(s => s.Rater.FullName))
                .ForMember(d => d.RaterImageUrl, o => o.MapFrom(s => s.Rater.ProfileImageUrl));

            // Hospital
            CreateMap<Hospital, HospitalDto>();
            CreateMap<CreateHospitalDto, Hospital>();

            // BloodBank
            CreateMap<BloodBank, BloodBankDto>();
            CreateMap<BloodBankStock, BloodStockDto>()
                .ForMember(d => d.BloodGroupDisplay, o => o.MapFrom(s => GetBloodGroupDisplay(s.BloodGroup)));

            // Chat
            CreateMap<ChatMessage, ChatMessageDto>()
                .ForMember(d => d.SenderName, o => o.MapFrom(s => s.Sender.FullName))
                .ForMember(d => d.SenderImageUrl, o => o.MapFrom(s => s.Sender.ProfileImageUrl));

            // Notification
            CreateMap<Notification, NotificationDto>();

            // Certificate
            CreateMap<DonationCertificate, CertificateDto>();

            // SuccessStory
            CreateMap<SuccessStory, SuccessStoryDto>()
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => s.User.FullName))
                .ForMember(d => d.AuthorImageUrl, o => o.MapFrom(s => s.User.ProfileImageUrl));
        }

        private static string GetBloodGroupDisplay(BloodGroup bloodGroup)
        {
            return bloodGroup switch
            {
                BloodGroup.APositive => "A+",
                BloodGroup.ANegative => "A-",
                BloodGroup.BPositive => "B+",
                BloodGroup.BNegative => "B-",
                BloodGroup.ABPositive => "AB+",
                BloodGroup.ABNegative => "AB-",
                BloodGroup.OPositive => "O+",
                BloodGroup.ONegative => "O-",
                _ => bloodGroup.ToString()
            };
        }
    }
}