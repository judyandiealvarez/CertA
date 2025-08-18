using System.ComponentModel.DataAnnotations;

namespace CertA.Models
{
    public enum CertificateStatus
    {
        Pending,
        Issued,
        Revoked,
        Expired
    }

    public enum CertificateType
    {
        Server,
        Client
    }

    public class CertificateEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string CommonName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? SubjectAlternativeNames { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        public DateTime IssuedDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        [Required]
        public CertificateStatus Status { get; set; } = CertificateStatus.Pending;

        [Required]
        public CertificateType Type { get; set; } = CertificateType.Server;

        [Required]
        public string CertificatePem { get; set; } = string.Empty;

        [Required]
        public string PublicKeyPem { get; set; } = string.Empty;

        [Required]
        public string PrivateKeyPem { get; set; } = string.Empty;
    }
}


