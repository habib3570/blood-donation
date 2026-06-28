namespace BloodDonationSystem.Application.DTOs.Hospital
{
    public class HospitalDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmergencyNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsOpen24Hours { get; set; }
        public string? OpenTime { get; set; }
        public string? CloseTime { get; set; }
        public bool HasBloodBank { get; set; }
        public bool IsCurrentlyOpen { get; set; }
        public double? DistanceKm { get; set; }
    }
}