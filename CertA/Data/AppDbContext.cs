using CertA.Models;
using Microsoft.EntityFrameworkCore;

namespace CertA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CertificateEntity> Certificates { get; set; }
        public DbSet<CertificateAuthority> CertificateAuthorities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CertificateEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CommonName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CertificatePem).IsRequired();
                entity.Property(e => e.PublicKeyPem).IsRequired();
                entity.Property(e => e.PrivateKeyPem).IsRequired();
            });
            
            modelBuilder.Entity<CertificateAuthority>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CommonName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Organization).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Country).IsRequired().HasMaxLength(2);
                entity.Property(e => e.State).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Locality).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CertificatePem).IsRequired();
                entity.Property(e => e.PrivateKeyPem).IsRequired();
            });
        }
    }
}


