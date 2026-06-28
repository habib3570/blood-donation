namespace BloodDonationSystem.Application.DTOs.Hospital
{
    public class HospitalFilterDto
    {
        public string? Name { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public bool? HasBloodBank { get; set; }
    }
}