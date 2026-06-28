namespace BloodDonationSystem.Application.DTOs.BloodBank
{
    public class BloodBankDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<BloodStockDto> Stocks { get; set; } = new();
        public double? DistanceKm { get; set; }
    }
}