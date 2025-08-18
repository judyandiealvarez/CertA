using CertA.Data;
using CertA.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Numerics;

namespace CertA.Services
{
    public interface ICertificateService
    {
        Task<List<CertificateEntity>> ListAsync();
        Task<CertificateEntity?> GetAsync(int id);
        Task<CertificateEntity> CreateAsync(string commonName, string? sans, CertificateType type);
        Task<byte[]> GetPrivateKeyPemAsync(int id);
        Task<byte[]> GetPublicKeyPemAsync(int id);
        Task<byte[]> GetCertificatePemAsync(int id);
        Task<byte[]> GetPfxAsync(int id, string password);
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

        public Task<List<CertificateEntity>> ListAsync()
        {
            return _db.Certificates
                .OrderByDescending(c => c.Id)
                .ToListAsync();
        }

        public Task<CertificateEntity?> GetAsync(int id)
        {
            return _db.Certificates.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CertificateEntity> CreateAsync(string commonName, string? sans, CertificateType type)
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
            var signedCertificate = await _caService.SignCertificateAsync(request, commonName, sans);
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
                PrivateKeyPem = privateKeyPem
            };

            _db.Certificates.Add(entity);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created CA-signed certificate {Serial} for {CN}", serialNumber, commonName);
            return entity;
        }

        public async Task<byte[]> GetPrivateKeyPemAsync(int id)
        {
            var cert = await _db.Certificates.FirstOrDefaultAsync(c => c.Id == id);
            if (cert?.PrivateKeyPem == null) throw new InvalidOperationException("Certificate or private key not found");
            return Encoding.UTF8.GetBytes(cert.PrivateKeyPem);
        }

        public async Task<byte[]> GetPublicKeyPemAsync(int id)
        {
            var cert = await _db.Certificates.FirstOrDefaultAsync(c => c.Id == id);
            if (cert?.PublicKeyPem == null) throw new InvalidOperationException("Certificate or public key not found");
            return Encoding.UTF8.GetBytes(cert.PublicKeyPem);
        }

        public async Task<byte[]> GetCertificatePemAsync(int id)
        {
            var cert = await _db.Certificates.FirstOrDefaultAsync(c => c.Id == id);
            if (cert?.CertificatePem == null) throw new InvalidOperationException("Certificate not found");
            return Encoding.UTF8.GetBytes(cert.CertificatePem);
        }

        public async Task<byte[]> GetPfxAsync(int id, string password)
        {
            var cert = await _db.Certificates.FirstOrDefaultAsync(c => c.Id == id);
            if (cert?.CertificatePem == null || cert?.PrivateKeyPem == null)
                throw new InvalidOperationException("Certificate or private key not found");

            try
            {
                // Create X509Certificate2 from PEM
                var certificate = X509Certificate2.CreateFromPem(cert.CertificatePem, cert.PrivateKeyPem);
                
                // Export as PKCS#12
                var pfxBytes = certificate.Export(X509ContentType.Pfx, password);
                
                _logger.LogInformation("Generated PFX for certificate {Id}", id);
                return pfxBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate PFX for certificate {Id}", id);
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


