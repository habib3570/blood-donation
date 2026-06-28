namespace BloodDonationSystem.Application.DTOs.Location
{
    public class DistrictDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
    }

    public class UpazilaDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DistrictId { get; set; }
    }
}