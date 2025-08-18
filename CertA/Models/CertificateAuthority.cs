using System.ComponentModel.DataAnnotations;

namespace CertA.Models
{
    public class CertificateAuthority
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string CommonName { get; set; } = string.Empty;
        
        [Required]
        public string Organization { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        public string State { get; set; } = string.Empty;
        
        [Required]
        public string Locality { get; set; } = string.Empty;
        
        [Required]
        public string CertificatePem { get; set; } = string.Empty;
        
        [Required]
        public string PrivateKeyPem { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpiryDate { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
