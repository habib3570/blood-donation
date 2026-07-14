using BloodDonationSystem.Application.Common.Interfaces;
using BloodDonationSystem.Application.Common.Models;
using BloodDonationSystem.Application.DTOs.Certificate;
using BloodDonationSystem.Application.Interfaces.Repositories;
using BloodDonationSystem.Application.Interfaces.Services;
using BloodDonationSystem.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BloodDonationSystem.Application.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CertificateService(
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork)
        {
            _certificateRepository = certificateRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<byte[]>> GenerateCertificateAsync(int donationId)
        {
            var donation = await _certificateRepository.GetDonationWithDetailsAsync(donationId);
            if (donation == null)
                return Result<byte[]>.Failure("Donation not found.");

            
            var certificate = donation.Certificate;
            if (certificate == null)
            {
                certificate = new DonationCertificate
                {
                    DonationId = donation.Id,
                    CertificateNumber = $"BDC-{DateTime.UtcNow:yyyyMMdd}-{donation.Id:D6}",
                    DonorName = donation.DonorProfile.User.FullName,
                    HospitalName = donation.HospitalName,
                    DonationDate = donation.DonationDate,
                    GeneratedAt = DateTime.UtcNow
                };
                await _certificateRepository.AddAsync(certificate);
                donation.CertificateGenerated = true;
                await _unitOfWork.SaveChangesAsync();
            }

            var pdfBytes = GeneratePdf(certificate);
            return Result<byte[]>.Success(pdfBytes);
        }

        public async Task<Result<CertificateDto>> GetCertificateByDonationIdAsync(int donationId)
        {
            var certificate = await _certificateRepository.GetByDonationIdAsync(donationId);
            if (certificate == null)
                return Result<CertificateDto>.Failure("Certificate not found.");

            return Result<CertificateDto>.Success(MapToDto(certificate));
        }

        public async Task<Result<List<CertificateDto>>> GetUserCertificatesAsync(int donorProfileId)
        {
            var certificates = await _certificateRepository.GetByDonorProfileIdAsync(donorProfileId);
            var dtos = certificates.Select(MapToDto).ToList();
            return Result<List<CertificateDto>>.Success(dtos);
        }

        private static CertificateDto MapToDto(DonationCertificate c) => new()
        {
            Id = c.Id,
            DonationId = c.DonationId,
            CertificateNumber = c.CertificateNumber,
            DonorName = c.DonorName,
            HospitalName = c.HospitalName,
            DonationDate = c.DonationDate,
            GeneratedAt = c.GeneratedAt
        };

        private static byte[] GeneratePdf(DonationCertificate cert)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(14));

                    page.Content().Column(col =>
                    {
                        col.Item().AlignCenter().Text("Certificate of Blood Donation")
                            .FontSize(28).Bold().FontColor(Colors.Red.Darken2);

                        col.Item().PaddingTop(30).AlignCenter()
                            .Text("This is to certify that").FontSize(16);

                        col.Item().PaddingTop(10).AlignCenter()
                            .Text(cert.DonorName).FontSize(24).Bold();

                        col.Item().PaddingTop(10).AlignCenter()
                            .Text($"has generously donated blood at {cert.HospitalName}")
                            .FontSize(16);

                        col.Item().PaddingTop(5).AlignCenter()
                            .Text($"on {cert.DonationDate:dd MMMM yyyy}")
                            .FontSize(16);

                        col.Item().PaddingTop(40).AlignCenter()
                            .Text("Thank you for saving lives! 🩸")
                            .FontSize(14).Italic();

                        col.Item().PaddingTop(30).AlignRight()
                            .Text($"Certificate No: {cert.CertificateNumber}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}