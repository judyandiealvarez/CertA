# CertA Architecture Documentation

## Overview

CertA is a complete Certification Authority (CA) system built with ASP.NET Core that provides certificate management capabilities through a web interface and REST API.

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    CertA Certification Authority            │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Web UI    │  │   REST API  │  │   Health    │         │
│  │  (Razor)    │  │  (Controllers)│  │   Checks   │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
├─────────────────────────────────────────────────────────────┤
│                    Business Logic Layer                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │Certificate  │  │Certificate  │  │   Models    │         │
│  │  Service    │  │Authority    │  │             │         │
│  │             │  │  Service    │  │             │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
├─────────────────────────────────────────────────────────────┤
│                    Data Access Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │ Entity      │  │ PostgreSQL  │  │   Migrations│         │
│  │ Framework   │  │  Database   │  │             │         │
│  │   Core      │  │             │  │             │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
└─────────────────────────────────────────────────────────────┘
```

## Component Architecture

### 1. Presentation Layer

#### Web UI (Razor Views)
- **Technology**: ASP.NET Core MVC with Razor Views
- **Framework**: Bootstrap 5 for responsive design
- **Features**:
  - Certificate management interface
  - CA information display
  - Download functionality
  - Form validation

#### REST API
- **Technology**: ASP.NET Core Web API
- **Format**: JSON responses
- **Authentication**: None (for development)
- **Endpoints**: Certificate CRUD operations, CA management

### 2. Business Logic Layer

#### CertificateService
```csharp
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
```

**Responsibilities:**
- Certificate lifecycle management
- Key generation and storage
- Certificate signing
- File format conversion (PEM/PFX)

#### CertificateAuthorityService
```csharp
public interface ICertificateAuthorityService
{
    Task<CertificateAuthority?> GetActiveCAAsync();
    Task<CertificateAuthority> CreateRootCAAsync(string name, string commonName, string organization, string country, string state, string locality);
    Task<X509Certificate2> SignCertificateAsync(CertificateRequest request, string commonName, string? sans);
}
```

**Responsibilities:**
- Root CA management
- Certificate signing
- CA certificate generation
- Trust chain establishment

### 3. Data Layer

#### Entity Framework Core
- **Provider**: Npgsql (PostgreSQL)
- **Configuration**: Code-first approach
- **Migrations**: Automatic database creation

#### Database Schema

```sql
-- Certificate Authorities
CREATE TABLE "CertificateAuthorities" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(255) NOT NULL,
    "CommonName" VARCHAR(255) NOT NULL,
    "Organization" VARCHAR(255) NOT NULL,
    "Country" VARCHAR(2) NOT NULL,
    "State" VARCHAR(255) NOT NULL,
    "Locality" VARCHAR(255) NOT NULL,
    "CertificatePem" TEXT NOT NULL,
    "PrivateKeyPem" TEXT NOT NULL,
    "CreatedDate" TIMESTAMP NOT NULL,
    "ExpiryDate" TIMESTAMP NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT true
);

-- Certificates
CREATE TABLE "Certificates" (
    "Id" SERIAL PRIMARY KEY,
    "CommonName" VARCHAR(255) NOT NULL,
    "SubjectAlternativeNames" TEXT,
    "SerialNumber" VARCHAR(50) NOT NULL,
    "IssuedDate" TIMESTAMP NOT NULL,
    "ExpiryDate" TIMESTAMP NOT NULL,
    "Status" INTEGER NOT NULL,
    "Type" INTEGER NOT NULL,
    "CertificatePem" TEXT NOT NULL,
    "PublicKeyPem" TEXT NOT NULL,
    "PrivateKeyPem" TEXT NOT NULL
);
```

## Certificate Architecture

### CA Hierarchy

```
CertA Root CA (4096-bit RSA)
├── Self-signed with CA extensions
├── 10-year validity period
├── Key Usage: KeyCertSign, CRLSign, DigitalSignature
├── Basic Constraints: CA=true, pathlen=0
└── Subject Key Identifier: SHA-1 hash of public key
```

### Issued Certificates

```
End Entity Certificate (2048-bit RSA)
├── Signed by Root CA
├── 1-year validity period
├── Key Usage: DigitalSignature, KeyEncipherment
├── Extended Key Usage: Server Authentication
├── Subject Alternative Names (SAN) support
└── Basic Constraints: CA=false
```

### Certificate Extensions

#### Root CA Extensions
```csharp
// Basic Constraints
request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));

// Key Usage
request.CertificateExtensions.Add(new X509KeyUsageExtension(
    X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.DigitalSignature, true));

// Subject Key Identifier
request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
```

#### End Entity Extensions
```csharp
// Basic Constraints
request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));

// Key Usage
request.CertificateExtensions.Add(new X509KeyUsageExtension(
    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));

// Extended Key Usage
request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
    new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, true)); // Server Authentication

// Subject Alternative Names
if (!string.IsNullOrEmpty(sans))
{
    var sanBuilder = new SubjectAlternativeNameBuilder();
    foreach (var san in sanList)
    {
        if (Uri.IsWellFormedUriString(san, UriKind.Absolute))
            sanBuilder.AddUri(new Uri(san));
        else
            sanBuilder.AddDnsName(san);
    }
    request.CertificateExtensions.Add(sanBuilder.Build());
}
```

## Security Architecture

### Cryptographic Implementation

#### Key Generation
```csharp
// Root CA: 4096-bit RSA
using var rsa = RSA.Create(4096);

// End Entity: 2048-bit RSA
using var rsa = RSA.Create(2048);
```

#### Certificate Signing
```csharp
// Create certificate request
var request = new CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

// Sign with CA
var signedCert = request.Create(caCert, notBefore, notAfter, serialNumber.ToByteArray());
```

#### Serial Number Generation
```csharp
private static BigInteger GenerateSerialNumber()
{
    var random = new byte[20];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(random);
    return new BigInteger(random, true, true);
}
```

### Data Protection

#### Private Key Storage
- **Format**: PEM encoded
- **Storage**: Database (encrypted in production)
- **Access**: Application-level encryption

#### Certificate Storage
- **Format**: PEM encoded
- **Storage**: Database
- **Access**: Read-only for downloads

## File Format Support

### PEM Format
```
-----BEGIN CERTIFICATE-----
MIIDXTCCAkWgAwIBAgIJA...
-----END CERTIFICATE-----

-----BEGIN RSA PRIVATE KEY-----
MIIEpAIBAAKCAQEA...
-----END RSA PRIVATE KEY-----

-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8A...
-----END PUBLIC KEY-----
```

### PKCS#12 (PFX) Format
```csharp
// Export certificate with private key
var pfxBytes = certificate.Export(X509ContentType.Pfx, password);
```

## Dependency Injection

### Service Registration
```csharp
// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<ICertificateAuthorityService, CertificateAuthorityService>();
```

### Service Lifetimes
- **Scoped**: Database context, business services
- **Singleton**: Configuration, logging
- **Transient**: Utility services

## Configuration Management

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=certa;Username=certa;Password=password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "CA": {
    "DefaultOrganization": "CertA Organization",
    "DefaultCountry": "US",
    "DefaultState": "California",
    "DefaultLocality": "San Francisco"
  }
}
```

### Environment-Specific Configuration
- **Development**: Local database, detailed logging
- **Production**: Secure database, minimal logging
- **Docker**: Containerized database, structured logging

## Error Handling

### Exception Handling Strategy
```csharp
try
{
    var certificate = await _service.CreateAsync(commonName, sans, type);
    return Ok(certificate);
}
catch (InvalidOperationException ex)
{
    _logger.LogError(ex, "Failed to create certificate");
    return BadRequest(new { error = ex.Message });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Unexpected error");
    return StatusCode(500, new { error = "Internal server error" });
}
```

### Logging Strategy
- **Information**: Certificate operations, CA activities
- **Warning**: Expiring certificates, configuration issues
- **Error**: Failed operations, security events
- **Debug**: Detailed operation tracing

## Performance Considerations

### Database Optimization
- **Indexes**: Serial number, common name, expiry date
- **Connection Pooling**: Entity Framework Core default
- **Query Optimization**: Eager loading for related data

### Memory Management
- **Disposal**: Proper disposal of cryptographic objects
- **Caching**: Certificate validation results
- **Streaming**: Large file downloads

### Scalability
- **Horizontal Scaling**: Stateless application design
- **Database Scaling**: Read replicas for reporting
- **Load Balancing**: Multiple application instances

## Monitoring and Observability

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));
```

### Metrics
- Certificate creation rate
- CA operations
- Error rates
- Response times

### Logging
- Structured logging with Serilog
- Correlation IDs for request tracing
- Security event logging

## Future Architecture Considerations

### Planned Enhancements
1. **ACME Protocol Support**
   - RFC 8555 compliance
   - HTTP-01 and DNS-01 challenges
   - Automated certificate issuance

2. **Certificate Revocation**
   - Certificate Revocation Lists (CRL)
   - Online Certificate Status Protocol (OCSP)
   - Revocation reason codes

3. **Intermediate CAs**
   - Multi-level CA hierarchy
   - CA certificate chaining
   - Cross-certification support

4. **High Availability**
   - Database clustering
   - Application load balancing
   - Geographic distribution

### Security Enhancements
1. **Hardware Security Modules (HSM)**
   - CA key protection
   - Cryptographic acceleration
   - Key backup and recovery

2. **Authentication and Authorization**
   - Role-based access control
   - Multi-factor authentication
   - Audit logging

3. **Network Security**
   - TLS 1.3 enforcement
   - Certificate pinning
   - Security headers

## Integration Points

### External Systems
- **Web Servers**: Apache, Nginx, IIS
- **Load Balancers**: HAProxy, F5, AWS ALB
- **Cloud Platforms**: AWS, Azure, GCP
- **Monitoring**: Prometheus, Grafana, ELK Stack

### APIs and Protocols
- **REST API**: Certificate management
- **ACME Protocol**: Automated certificate issuance
- **SCEP Protocol**: Simple Certificate Enrollment Protocol
- **EST Protocol**: Enrollment over Secure Transport

## Compliance and Standards

### X.509 Standards
- **RFC 5280**: Internet X.509 Public Key Infrastructure
- **RFC 8555**: Automatic Certificate Management Environment (ACME)
- **RFC 6960**: X.509 Internet Public Key Infrastructure Online Certificate Status Protocol

### Security Standards
- **FIPS 140-2**: Cryptographic module validation
- **Common Criteria**: Security evaluation
- **SOC 2**: Security, availability, and confidentiality

## Conclusion

CertA provides a solid foundation for certificate management with a clean, layered architecture that supports both current requirements and future enhancements. The modular design allows for easy extension and maintenance while maintaining security and performance standards.
