using System.ComponentModel.DataAnnotations;

namespace BloodDonationSystem.Application.DTOs.Hospital
{
    public class CreateHospitalDto
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string District { get; set; } = string.Empty;
        [Required] public string Upazila { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmergencyNumber { get; set; }
        [Required] public double Latitude { get; set; }
        [Required] public double Longitude { get; set; }
        public bool IsOpen24Hours { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public bool HasBloodBank { get; set; }
    }
}