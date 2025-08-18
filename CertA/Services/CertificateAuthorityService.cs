using CertA.Data;
using CertA.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Numerics;

namespace CertA.Services
{
    public interface ICertificateAuthorityService
    {
        Task<CertificateAuthority?> GetActiveCAAsync();
        Task<CertificateAuthority> CreateRootCAAsync(string name, string commonName, string organization, string country, string state, string locality);
        Task<X509Certificate2> SignCertificateAsync(CertificateRequest request, string commonName, string? sans);
    }

    public class CertificateAuthorityService : ICertificateAuthorityService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CertificateAuthorityService> _logger;

        public CertificateAuthorityService(AppDbContext db, ILogger<CertificateAuthorityService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<CertificateAuthority?> GetActiveCAAsync()
        {
            return await _db.CertificateAuthorities
                .Where(ca => ca.IsActive)
                .OrderByDescending(ca => ca.CreatedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<CertificateAuthority> CreateRootCAAsync(string name, string commonName, string organization, string country, string state, string locality)
        {
            // Generate CA key pair
            using var rsa = RSA.Create(4096);
            var privateKeyPem = rsa.ExportRSAPrivateKeyPem();
            var publicKeyPem = rsa.ExportRSAPublicKeyPem();

            // Create CA certificate
            var notBefore = DateTime.UtcNow;
            var notAfter = notBefore.AddYears(10); // CA certs typically last longer
            var serialNumber = GenerateSerialNumber();

            var subject = new X500DistinguishedName($"CN={commonName},O={organization},L={locality},ST={state},C={country}");
            var issuer = subject; // Self-signed for root CA

            var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            
            // Add CA extensions
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature, true));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            // Create the CA certificate
            var caCertificate = request.CreateSelfSigned(notBefore, notAfter);
            var certificatePem = caCertificate.ExportCertificatePem();

            var ca = new CertificateAuthority
            {
                Name = name,
                CommonName = commonName,
                Organization = organization,
                Country = country,
                State = state,
                Locality = locality,
                CertificatePem = certificatePem,
                PrivateKeyPem = privateKeyPem,
                CreatedDate = notBefore,
                ExpiryDate = notAfter,
                IsActive = true
            };

            _db.CertificateAuthorities.Add(ca);
            await _db.SaveChangesAsync();
            
            _logger.LogInformation("Created root CA {Name} with serial {Serial}", name, serialNumber);
            return ca;
        }

        public async Task<X509Certificate2> SignCertificateAsync(CertificateRequest request, string commonName, string? sans)
        {
            var ca = await GetActiveCAAsync();
            if (ca == null)
                throw new InvalidOperationException("No active Certificate Authority found");

            // Load CA certificate and private key
            var caCert = X509Certificate2.CreateFromPem(ca.CertificatePem, ca.PrivateKeyPem);
            var caPrivateKey = caCert.GetRSAPrivateKey();
            if (caPrivateKey == null)
                throw new InvalidOperationException("CA private key not found");

            // Add SAN extension if provided
            if (!string.IsNullOrEmpty(sans))
            {
                var sanBuilder = new SubjectAlternativeNameBuilder();
                var sanList = sans.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s));
                
                foreach (var san in sanList)
                {
                    if (Uri.IsWellFormedUriString(san, UriKind.Absolute))
                    {
                        sanBuilder.AddUri(new Uri(san));
                    }
                    else
                    {
                        sanBuilder.AddDnsName(san);
                    }
                }
                
                request.CertificateExtensions.Add(sanBuilder.Build());
            }

            // Add basic constraints for end entity
            request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
            
            // Add key usage
            request.CertificateExtensions.Add(new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
            
            // Add extended key usage for server authentication
            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, true)); // Server Authentication

            // Sign the certificate
            var notBefore = DateTime.UtcNow;
            var notAfter = notBefore.AddYears(1);
            var serialNumber = GenerateSerialNumber();

            var signedCert = request.Create(caCert, notBefore, notAfter, serialNumber.ToByteArray());
            
            _logger.LogInformation("Signed certificate for {CN} with serial {Serial}", commonName, serialNumber);
            return signedCert;
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
