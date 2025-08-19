using System.ComponentModel.DataAnnotations;

namespace CertA.Models
{
    public enum CertificateStatus
    {
        Pending = 0,
        Issued = 1,
        Revoked = 2,
        Expired = 3
    }

    public enum CertificateType
    {
        Server = 0,
        Client = 1,
        CodeSigning = 2,
        Email = 3
    }

    public class CertificateEntity
    {
        public int Id { get; set; }

        [Required]
        public string CommonName { get; set; } = string.Empty;

        public string? SubjectAlternativeNames { get; set; }

        [Required]
        public string SerialNumber { get; set; } = string.Empty;

        public DateTime IssuedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public CertificateStatus Status { get; set; }

        public CertificateType Type { get; set; }

        [Required]
        public string CertificatePem { get; set; } = string.Empty;

        [Required]
        public string PublicKeyPem { get; set; } = string.Empty;

        [Required]
        public string PrivateKeyPem { get; set; } = string.Empty;

        // User ownership
        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }
    }
}


