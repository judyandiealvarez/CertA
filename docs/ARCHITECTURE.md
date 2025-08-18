# CertA Architecture Documentation

Comprehensive technical architecture documentation for the CertA Certification Authority system.

## 🏗️ System Overview

CertA is a multi-layered web application that provides certificate authority services with user authentication and authorization. The system follows a clean architecture pattern with clear separation of concerns.

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
├─────────────────────────────────────────────────────────────┤
│  Web Interface (ASP.NET Core MVC)                           │
│  ├── Controllers (MVC Pattern)                              │
│  ├── Views (Razor Pages)                                    │
│  └── Static Assets (CSS, JS, Images)                        │
├─────────────────────────────────────────────────────────────┤
│                   Business Logic Layer                       │
├─────────────────────────────────────────────────────────────┤
│  Services                                                    │
│  ├── CertificateService                                     │
│  ├── CertificateAuthorityService                            │
│  └── Identity Services (ASP.NET Core Identity)              │
├─────────────────────────────────────────────────────────────┤
│                     Data Access Layer                        │
├─────────────────────────────────────────────────────────────┤
│  Entity Framework Core                                      │
│  ├── AppDbContext                                           │
│  ├── Models                                                 │
│  └── Migrations                                             │
├─────────────────────────────────────────────────────────────┤
│                     Data Storage Layer                       │
├─────────────────────────────────────────────────────────────┤
│  PostgreSQL Database                                        │
│  ├── Identity Tables (AspNetUsers, AspNetRoles, etc.)       │
│  ├── Certificate Tables (Certificates, CertificateAuthorities) │
│  └── Application Data                                       │
└─────────────────────────────────────────────────────────────┘
```

## 🔐 Authentication & Authorization Architecture

### Identity System

CertA uses ASP.NET Core Identity for user management and authentication:

#### Core Components
- **ApplicationUser**: Custom user model extending IdentityUser
- **UserManager**: Manages user operations (create, update, delete)
- **SignInManager**: Handles authentication (login, logout)
- **IdentityDbContext**: Database context for identity tables

#### User Model
```csharp
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Organization { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
```

#### Authentication Flow
1. **Registration**: User creates account with email/password
2. **Login**: User authenticates with credentials
3. **Session Management**: Cookie-based sessions with configurable timeout
4. **Authorization**: Role-based access control (future enhancement)

### Authorization Model

#### User Isolation
- **Certificate Ownership**: Each certificate belongs to a specific user
- **Data Separation**: Users can only access their own certificates
- **Profile Access**: Users can only manage their own profile

#### Access Control Matrix

| Resource | Public Access | Authenticated Users | Owner Only |
|----------|---------------|-------------------|------------|
| CA Information | ✅ | ✅ | - |
| Root CA Download | ✅ | ✅ | - |
| Certificate List | ❌ | ✅ (own only) | ✅ |
| Certificate Details | ❌ | ✅ (own only) | ✅ |
| Certificate Downloads | ❌ | ✅ (own only) | ✅ |
| Profile Management | ❌ | ✅ (own only) | ✅ |

## 📜 Certificate Architecture

### Certificate Hierarchy

```
CertA Root CA (4096-bit RSA, 10-year validity)
├── Self-signed with CA extensions
├── Basic Constraints: CA=true, pathlen=0
├── Key Usage: KeyCertSign, CrlSign, DigitalSignature
└── Subject Key Identifier: SHA-1 hash of public key
    │
    └── Issued Certificates (2048-bit RSA, 1-year validity)
        ├── Signed by Root CA
        ├── Basic Constraints: CA=false
        ├── Key Usage: DigitalSignature, KeyEncipherment
        ├── Extended Key Usage: Server Authentication (1.3.6.1.5.5.7.3.1)
        └── Subject Alternative Names: Multiple domains/IPs
```

### Certificate Types

#### Server Certificates
- **Purpose**: Web servers, APIs, load balancers
- **Key Usage**: DigitalSignature, KeyEncipherment
- **Extended Key Usage**: Server Authentication
- **Validity**: 1 year
- **Key Size**: 2048-bit RSA

#### Client Certificates
- **Purpose**: Client authentication, VPN connections
- **Key Usage**: DigitalSignature, KeyEncipherment
- **Extended Key Usage**: Client Authentication
- **Validity**: 1 year
- **Key Size**: 2048-bit RSA

#### Code Signing Certificates
- **Purpose**: Software and application signing
- **Key Usage**: DigitalSignature
- **Extended Key Usage**: Code Signing
- **Validity**: 1 year
- **Key Size**: 2048-bit RSA

#### Email Certificates
- **Purpose**: Email encryption and digital signatures
- **Key Usage**: DigitalSignature, KeyEncipherment
- **Extended Key Usage**: Email Protection
- **Validity**: 1 year
- **Key Size**: 2048-bit RSA

### Certificate Extensions

#### Standard Extensions
- **Basic Constraints**: Defines CA vs end-entity certificates
- **Key Usage**: Specifies allowed key operations
- **Extended Key Usage**: Defines certificate purpose
- **Subject Key Identifier**: Unique identifier for the public key
- **Subject Alternative Names**: Multiple domain names and IP addresses

#### Custom Extensions
- **Authority Key Identifier**: Links to CA certificate
- **Certificate Policies**: Defines certificate usage policies
- **CRL Distribution Points**: Points to certificate revocation lists

## 🗄️ Database Architecture

### Entity Relationship Diagram

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   AspNetUsers   │    │   Certificates  │    │CertificateAuth. │
├─────────────────┤    ├─────────────────┤    ├─────────────────┤
│ Id (PK)         │    │ Id (PK)         │    │ Id (PK)         │
│ UserName        │    │ CommonName      │    │ Name            │
│ Email           │    │ SubjectAltNames │    │ CommonName      │
│ FirstName       │    │ SerialNumber    │    │ Organization    │
│ LastName        │    │ IssuedDate      │    │ Country         │
│ Organization    │    │ ExpiryDate      │    │ State           │
│ CreatedDate     │    │ Status          │    │ Locality        │
│ IsActive        │    │ Type            │    │ CertificatePem  │
│ PasswordHash    │    │ CertificatePem  │    │ PrivateKeyPem   │
│ ...             │    │ PublicKeyPem    │    │ CreatedDate     │
└─────────────────┘    │ PrivateKeyPem   │    │ ExpiryDate      │
         │              │ UserId (FK)     │    │ IsActive        │
         │              └─────────────────┘    └─────────────────┘
         │                       │
         └───────────────────────┘
                    (1:N Relationship)
```

### Database Schema

#### Identity Tables (ASP.NET Core Identity)
- **AspNetUsers**: User accounts and profile information
- **AspNetUserClaims**: User claims and permissions
- **AspNetUserLogins**: External login providers
- **AspNetUserTokens**: User tokens for password reset, etc.
- **AspNetRoles**: Role definitions (future use)
- **AspNetUserRoles**: User-role assignments (future use)
- **AspNetRoleClaims**: Role-based claims (future use)

#### Application Tables
- **Certificates**: User certificates with ownership
- **CertificateAuthorities**: Root CA information

### Data Relationships

#### User-Certificate Relationship
- **One-to-Many**: One user can have multiple certificates
- **Foreign Key**: `Certificates.UserId` references `AspNetUsers.Id`
- **Cascade Delete**: When user is deleted, their certificates are deleted
- **User Isolation**: Users can only access their own certificates

#### Certificate Authority
- **Singleton Pattern**: Only one active CA at a time
- **Self-Contained**: CA certificate and private key stored in database
- **Public Access**: CA information accessible without authentication

## 🔧 Service Layer Architecture

### Service Interfaces

#### ICertificateService
```csharp
public interface ICertificateService
{
    Task<List<CertificateEntity>> ListAsync(string userId);
    Task<CertificateEntity?> GetAsync(int id, string userId);
    Task<CertificateEntity> CreateAsync(string commonName, string? sans, CertificateType type, string userId);
    Task<byte[]> GetPrivateKeyPemAsync(int id, string userId);
    Task<byte[]> GetPublicKeyPemAsync(int id, string userId);
    Task<byte[]> GetCertificatePemAsync(int id, string userId);
    Task<byte[]> GetPfxAsync(int id, string password, string userId);
}
```

#### ICertificateAuthorityService
```csharp
public interface ICertificateAuthorityService
{
    Task<CertificateAuthority?> GetActiveCAAsync();
    Task<CertificateAuthority> CreateRootCAAsync(string name, string commonName, string organization, string country, string state, string locality);
    Task<X509Certificate2> SignCertificateAsync(CertificateRequest request, string commonName, string? sans);
}
```

### Service Responsibilities

#### CertificateService
- **Certificate CRUD**: Create, read, update, delete certificates
- **User Filtering**: Ensure users only access their own certificates
- **File Generation**: Generate PEM and PFX files for download
- **Validation**: Validate certificate parameters and user permissions

#### CertificateAuthorityService
- **CA Management**: Create and manage root CA
- **Certificate Signing**: Sign certificate requests with CA private key
- **CA Information**: Provide CA details for public access
- **Key Management**: Handle CA private key securely

## 🌐 Web Interface Architecture

### MVC Pattern

#### Controllers
- **AccountController**: User authentication and profile management
- **CertificatesController**: Certificate management with authorization
- **HomeController**: Dashboard and public pages

#### Views
- **Account Views**: Login, register, profile management
- **Certificate Views**: List, create, details, download
- **Shared Views**: Layout, navigation, common components

#### Models
- **View Models**: Data transfer objects for views
- **Entity Models**: Database entities
- **Validation Models**: Input validation and error handling

### Navigation Structure

```
Home (Public)
├── Login (Public)
├── Register (Public)
├── Certificate Authority (Public)
└── Dashboard (Authenticated)
    ├── My Certificates (Authenticated)
    │   ├── List Certificates
    │   ├── Create Certificate
    │   ├── View Details
    │   └── Download Files
    └── Profile (Authenticated)
        ├── View Profile
        ├── Update Profile
        └── Change Password
```

## 🔒 Security Architecture

### Authentication Security

#### Password Security
- **Hashing**: ASP.NET Core Identity password hashing
- **Requirements**: Minimum 6 characters with complexity
- **Validation**: Server-side and client-side validation
- **Storage**: Encrypted password hashes in database

#### Session Security
- **Cookie Configuration**: HTTP-only, secure cookies
- **Session Timeout**: 12-hour sessions with sliding expiration
- **CSRF Protection**: Anti-forgery token validation
- **Secure Headers**: Security headers for XSS protection

### Authorization Security

#### User Isolation
- **Data Separation**: Complete isolation of user data
- **Access Control**: Users can only access their own resources
- **Permission Validation**: Server-side validation of all requests
- **Audit Logging**: Log all certificate operations

#### Certificate Security
- **Private Key Protection**: Encrypted storage in database
- **Secure Downloads**: Protected download endpoints
- **Key Rotation**: Support for certificate renewal
- **Revocation Support**: Framework for certificate revocation

### Data Protection

#### Database Security
- **Connection Encryption**: TLS encryption for database connections
- **Access Control**: Database user with minimal required permissions
- **Backup Security**: Encrypted backups with secure storage
- **Audit Logging**: Database access logging

#### File System Security
- **Certificate Storage**: Secure storage of certificates and keys
- **Access Permissions**: Appropriate file permissions
- **Encryption**: Encryption at rest for sensitive data
- **Secure Deletion**: Secure deletion of temporary files

## 📊 Performance Architecture

### Caching Strategy
- **CA Certificate Caching**: Cache root CA certificate in memory
- **User Session Caching**: Efficient session management
- **Database Query Optimization**: Optimized queries with proper indexing
- **Static Asset Caching**: Browser caching for static resources

### Scalability Considerations
- **Database Connection Pooling**: Efficient database connections
- **Async Operations**: Non-blocking I/O operations
- **Memory Management**: Efficient memory usage for certificate operations
- **Load Balancing**: Support for horizontal scaling

### Monitoring and Logging
- **Application Logging**: Structured logging with Serilog
- **Performance Monitoring**: Response time and resource usage
- **Error Tracking**: Comprehensive error logging and tracking
- **Security Monitoring**: Authentication and authorization events

## 🔄 Deployment Architecture

### Container Architecture
- **Web Application**: ASP.NET Core application container
- **Database**: PostgreSQL container with persistent storage
- **Load Balancer**: Optional load balancer for high availability
- **Monitoring**: Optional monitoring and logging containers

### Environment Configuration
- **Development**: Local development with hot reload
- **Staging**: Pre-production testing environment
- **Production**: Production deployment with security hardening

### Security Hardening
- **Network Security**: Firewall rules and network isolation
- **Container Security**: Secure container configuration
- **Secret Management**: Secure handling of secrets and keys
- **Backup Strategy**: Regular backups with secure storage

## 🚀 Future Architecture Considerations

### Planned Enhancements
- **ACME Protocol**: RFC 8555 implementation for automated certificate management
- **Intermediate CAs**: Multi-level certificate authority hierarchy
- **Certificate Revocation**: CRL and OCSP responder implementation
- **API Authentication**: JWT token-based API access
- **Role-Based Access Control**: Advanced authorization with roles and permissions

### Scalability Improvements
- **Microservices**: Decompose into microservices for better scalability
- **Event-Driven Architecture**: Event sourcing for certificate lifecycle
- **Distributed Caching**: Redis-based distributed caching
- **Message Queues**: Asynchronous processing with message queues

### Security Enhancements
- **Hardware Security Modules**: HSM integration for key management
- **Multi-Factor Authentication**: MFA support for enhanced security
- **Certificate Transparency**: CT log integration for certificate monitoring
- **Advanced Encryption**: Post-quantum cryptography support

---

**CertA provides a solid foundation for certificate management with a clean, layered architecture that supports both current requirements and future enhancements. The modular design allows for easy extension and maintenance while maintaining security and performance standards.**

*Last updated: August 2025*
