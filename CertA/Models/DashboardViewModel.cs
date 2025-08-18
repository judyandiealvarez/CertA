using CertA.Models;

namespace CertA.Models
{
    public class DashboardViewModel
    {
        public int TotalCertificates { get; set; }
        public int IssuedCertificates { get; set; }
        public int PendingRequests { get; set; }
        public int ExpiredCertificates { get; set; }
    }
}
