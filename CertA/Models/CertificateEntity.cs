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
        Email = 3,
        Wildcard = 4
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

        // Helper property to check if this is a wildcard certificate
        public bool IsWildcard => CommonName.StartsWith("*.") || 
                                 (SubjectAlternativeNames?.Contains("*.") == true);

        // Helper method to get all domains (including wildcards)
        public IEnumerable<string> GetAllDomains()
        {
            var domains = new List<string> { CommonName };
            
            if (!string.IsNullOrEmpty(SubjectAlternativeNames))
            {
                domains.AddRange(SubjectAlternativeNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s)));
            }
            
            return domains.Distinct();
        }

        // Helper property to get HAProxy format description
        public string HAProxyFormatDescription => 
            "HAProxy format combines Private Key + Certificate + CA Certificate in a single PEM file for easy deployment.";
    }
}


