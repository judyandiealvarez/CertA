using CertA.Data;
using CertA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Numerics;

namespace CertA.Services
{
    public interface ICertificateService
    {
        Task<List<CertificateEntity>> ListAsync(string userId);
        Task<CertificateEntity?> GetAsync(int id, string userId);
        Task<CertificateEntity> CreateAsync(string commonName, string? sans, CertificateType type, string userId);
        Task<CertificateEntity> CreateWildcardAsync(string domain, string? additionalSans, string userId);
        Task<byte[]> GetPrivateKeyPemAsync(int id, string userId);
        Task<byte[]> GetPublicKeyPemAsync(int id, string userId);
        Task<byte[]> GetCertificatePemAsync(int id, string userId);
        Task<byte[]> GetPfxAsync(int id, string password, string userId);
        Task<List<CertificateEntity>> GetExpiringCertificatesAsync(int daysThreshold = 30);
    }

    public class CertificateService : ICertificateService
    {
        private readonly AppDbContext _db;
        private readonly ICertificateAuthorityService _caService;
        private readonly ILogger<CertificateService> _logger;

        public CertificateService(AppDbContext db, ICertificateAuthorityService caService, ILogger<CertificateService> logger)
        {
            _db = db;
            _caService = caService;
            _logger = logger;
        }

        public Task<List<CertificateEntity>> ListAsync(string userId)
        {
            return _db.Certificates
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public Task<CertificateEntity?> GetAsync(int id, string userId)
        {
            return _db.Certificates
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<CertificateEntity> CreateAsync(string commonName, string? sans, CertificateType type, string userId)
        {
            // Check if we have an active CA
            var ca = await _caService.GetActiveCAAsync();
            if (ca == null)
            {
                // Create a default CA if none exists
                ca = await _caService.CreateRootCAAsync(
                    "CertA Root CA",
                    "CertA Root CA",
                    "CertA Organization",
                    "US",
                    "California",
                    "San Francisco"
                );
            }

            // Generate a new key pair for the certificate
            using var rsa = RSA.Create(2048);
            var publicKeyPem = rsa.ExportRSAPublicKeyPem();
            var privateKeyPem = rsa.ExportRSAPrivateKeyPem();

            // Create certificate request
            var subject = new X500DistinguishedName($"CN={commonName},O=CertA Organization,C=US");
            var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Sign the certificate with our CA
            var signedCertificate = await _caService.SignCertificateAsync(request, commonName, sans, type);
            var certificatePem = signedCertificate.ExportCertificatePem();

            var notBefore = DateTime.UtcNow;
            var notAfter = notBefore.AddYears(1);
            var serialNumber = GenerateSerialNumber();

            var entity = new CertificateEntity
            {
                CommonName = commonName,
                SubjectAlternativeNames = sans,
                SerialNumber = serialNumber.ToString("X"),
                IssuedDate = notBefore,
                ExpiryDate = notAfter,
                Status = CertificateStatus.Issued,
                Type = type,
                CertificatePem = certificatePem,
                PublicKeyPem = publicKeyPem,
                PrivateKeyPem = privateKeyPem,
                UserId = userId
            };

            _db.Certificates.Add(entity);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created {Type} certificate {Serial} for {CN} by user {UserId}", type, serialNumber, commonName, userId);
            return entity;
        }

        public async Task<CertificateEntity> CreateWildcardAsync(string domain, string? additionalSans, string userId)
        {
            // Ensure domain doesn't start with wildcard
            if (domain.StartsWith("*."))
            {
                domain = domain.Substring(2);
            }

            // Create wildcard common name
            var wildcardCommonName = $"*.{domain}";

            // Combine additional SANs
            var allSans = new List<string> { wildcardCommonName };
            if (!string.IsNullOrEmpty(additionalSans))
            {
                allSans.AddRange(additionalSans.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s)));
            }

            var sansString = string.Join(",", allSans);

            return await CreateAsync(wildcardCommonName, sansString, CertificateType.Wildcard, userId);
        }

        public async Task<List<CertificateEntity>> GetExpiringCertificatesAsync(int daysThreshold = 30)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            
            return await _db.Certificates
                .Where(c => c.Status == CertificateStatus.Issued && 
                           c.ExpiryDate <= thresholdDate && 
                           c.ExpiryDate > DateTime.UtcNow)
                .OrderBy(c => c.ExpiryDate)
                .ToListAsync();
        }

        public async Task<byte[]> GetPrivateKeyPemAsync(int id, string userId)
        {
            var cert = await _db.Certificates
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();
            if (cert?.PrivateKeyPem == null) throw new InvalidOperationException("Certificate or private key not found");
            return Encoding.UTF8.GetBytes(cert.PrivateKeyPem);
        }

        public async Task<byte[]> GetPublicKeyPemAsync(int id, string userId)
        {
            var cert = await _db.Certificates
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();
            if (cert?.PublicKeyPem == null) throw new InvalidOperationException("Certificate or public key not found");
            return Encoding.UTF8.GetBytes(cert.PublicKeyPem);
        }

        public async Task<byte[]> GetCertificatePemAsync(int id, string userId)
        {
            var cert = await _db.Certificates
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();
            if (cert?.CertificatePem == null) throw new InvalidOperationException("Certificate not found");
            return Encoding.UTF8.GetBytes(cert.CertificatePem);
        }

        public async Task<byte[]> GetPfxAsync(int id, string password, string userId)
        {
            var cert = await _db.Certificates
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();
            if (cert?.CertificatePem == null || cert?.PrivateKeyPem == null)
                throw new InvalidOperationException("Certificate or private key not found");

            try
            {
                // Create X509Certificate2 from PEM
                var certificate = X509Certificate2.CreateFromPem(cert.CertificatePem, cert.PrivateKeyPem);

                // Export as PKCS#12
                var pfxBytes = certificate.Export(X509ContentType.Pfx, password);

                _logger.LogInformation("Generated PFX for certificate {Id} by user {UserId}", id, userId);
                return pfxBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate PFX for certificate {Id} by user {UserId}", id, userId);
                throw new InvalidOperationException("Failed to generate PFX file");
            }
        }

        private static BigInteger GenerateSerialNumber()
        {
            var random = new byte[20];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return new BigInteger(random, true, true);
        }
    }
}


