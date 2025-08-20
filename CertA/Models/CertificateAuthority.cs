using System.ComponentModel.DataAnnotations;

namespace CertA.Models
{
    public class CertificateAuthority
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string CommonName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Organization { get; set; } = string.Empty;

        [Required]
        [MaxLength(2)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Locality { get; set; } = string.Empty;

        [Required]
        public string CertificatePem { get; set; } = string.Empty;

        [Required]
        public string PrivateKeyPem { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Helper property to check if CA is expired
        public bool IsExpired => DateTime.UtcNow > ExpiryDate;

        // Helper property to check if CA is valid
        public bool IsValid => IsActive && !IsExpired;
    }
}
