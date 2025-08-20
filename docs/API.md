# CertA API Documentation

Complete REST API reference for the CertA Certification Authority system.

## 🔐 Authentication

All API endpoints require authentication unless otherwise specified. The system uses cookie-based authentication with ASP.NET Core Identity.

### Authentication Endpoints

#### User Registration
```http
POST /Account/Register
Content-Type: application/x-www-form-urlencoded
```

**Request Body:**
```json
{
  "FirstName": "John",
  "LastName": "Doe",
  "Email": "john.doe@example.com",
  "Organization": "Example Corp",
  "Password": "SecurePass123!",
  "ConfirmPassword": "SecurePass123!"
}
```

**Response:**
- `302 Found` - Redirects to dashboard on success
- `200 OK` - Returns registration form with validation errors

#### User Login
```http
POST /Account/Login
Content-Type: application/x-www-form-urlencoded
```

**Request Body:**
```json
{
  "Email": "john.doe@example.com",
  "Password": "SecurePass123!",
  "RememberMe": true
}
```

**Response:**
- `302 Found` - Redirects to dashboard on success
- `200 OK` - Returns login form with validation errors

#### User Logout
```http
POST /Account/Logout
```

**Response:**
- `302 Found` - Redirects to home page

#### User Profile
```http
GET /Account/Profile
```

**Response:**
- `200 OK` - Returns user profile page
- `302 Found` - Redirects to login if not authenticated

#### Update Profile
```http
POST /Account/Profile
Content-Type: application/x-www-form-urlencoded
```

**Request Body:**
```json
{
  "FirstName": "John",
  "LastName": "Smith",
  "Email": "john.doe@example.com",
  "Organization": "Updated Corp"
}
```

**Response:**
- `302 Found` - Redirects to profile page with success message
- `200 OK` - Returns profile form with validation errors

#### Change Password
```http
POST /Account/ChangePassword
Content-Type: application/x-www-form-urlencoded
```

**Request Body:**
```json
{
  "CurrentPassword": "OldPass123!",
  "NewPassword": "NewPass456!",
  "ConfirmPassword": "NewPass456!"
}
```

**Response:**
- `302 Found` - Redirects to profile page with success/error message

## 🔧 New Features

### Wildcard Certificate Support

The system now supports wildcard certificates for subdomain coverage.

#### Wildcard Certificate Creation
```http
POST /Certificates/Create
Content-Type: application/x-www-form-urlencoded
```

**Request Body for Wildcard Certificate:**
```json
{
  "CommonName": "example.com",
  "SubjectAlternativeNames": "www.example.com,api.example.com",
  "Type": "Wildcard"
}
```

**Response:**
- Creates `*.example.com` certificate
- Includes additional SANs if provided
- Validates wildcard format automatically

#### Wildcard Validation Rules
- Only one wildcard per certificate
- Must follow format: `*.domain.com`
- Cannot contain multiple wildcards
- Proper X.509 extensions applied

### Single CA Enforcement

The system enforces a single active Certificate Authority per server for security.

#### CA Management
- Only one active CA allowed
- Automatic CA creation on first certificate
- CA deactivation support
- Unique constraint on active CAs

### Data Protection

ASP.NET Core Data Protection keys are now stored in the database.

#### Benefits
- Multi-replica deployment support
- No file system dependencies
- Improved security and scalability
- Automatic key rotation

## 📜 Certificate Management

### List User Certificates
```http
GET /Certificates
```

**Authentication:** Required

**Response:**
- `200 OK` - Returns list of user's certificates
- `302 Found` - Redirects to login if not authenticated

**Example Response:**
```html
<!-- Returns HTML page with certificate list -->
```

### Create New Certificate
```http
GET /Certificates/Create
```

**Authentication:** Required

**Response:**
- `200 OK` - Returns certificate creation form

```http
POST /Certificates/Create
Content-Type: application/x-www-form-urlencoded
```

**Request Body:**
```json
{
  "CommonName": "example.com",
  "SubjectAlternativeNames": "www.example.com,api.example.com",
  "Type": "Server"
}
```

**Response:**
- `302 Found` - Redirects to certificate details on success
- `200 OK` - Returns form with validation errors

### View Certificate Details
```http
GET /Certificates/Details/{id}
```

**Authentication:** Required

**Parameters:**
- `id` (int) - Certificate ID

**Response:**
- `200 OK` - Returns certificate details page
- `404 Not Found` - Certificate not found or not owned by user

### Download Certificate Files

#### Certificate (PEM)
```http
GET /Certificates/DownloadCertificate/{id}
```

**Authentication:** Required

**Response:**
- `200 OK` - Returns certificate in PEM format
- `404 Not Found` - Certificate not found

**Headers:**
```
Content-Type: application/x-pem-file
Content-Disposition: attachment; filename="example.com_certificate.pem"
```

#### Private Key (PEM)
```http
GET /Certificates/DownloadPrivateKey/{id}
```

**Authentication:** Required

**Response:**
- `200 OK` - Returns private key in PEM format
- `404 Not Found` - Certificate not found

**Headers:**
```
Content-Type: application/x-pem-file
Content-Disposition: attachment; filename="example.com_private_key.pem"
```

#### Public Key (PEM)
```http
GET /Certificates/DownloadPublicKey/{id}
```

**Authentication:** Required

**Response:**
- `200 OK` - Returns public key in PEM format
- `404 Not Found` - Certificate not found

**Headers:**
```
Content-Type: application/x-pem-file
Content-Disposition: attachment; filename="example.com_public_key.pem"
```

#### Certificate (PFX/PKCS#12)
```http
GET /Certificates/DownloadPfx/{id}?password={password}
```

**Authentication:** Required

**Parameters:**
- `id` (int) - Certificate ID
- `password` (string) - PFX password (default: "password")

**Response:**
- `200 OK` - Returns certificate in PKCS#12 format
- `404 Not Found` - Certificate not found

**Headers:**
```
Content-Type: application/x-pkcs12
Content-Disposition: attachment; filename="example.com.pfx"
```

## 🏛️ Certificate Authority Management

### View CA Information
```http
GET /Certificates/Authority
```

**Authentication:** Not required (public access)

**Response:**
- `200 OK` - Returns CA information page
- `404 Not Found` - No CA found

### Download Root CA Certificate
```http
GET /Certificates/DownloadRootCA
```

**Authentication:** Not required (public access)

**Response:**
- `200 OK` - Returns root CA certificate in PEM format
- `404 Not Found` - No CA found

**Headers:**
```
Content-Type: application/x-pem-file
Content-Disposition: attachment; filename="CertA_Root_CA.pem"
```

### Download Root CA Certificate (PFX)
```http
GET /Certificates/DownloadRootCAPfx?password={password}
```

**Authentication:** Not required (public access)

**Parameters:**
- `password` (string) - PFX password (default: "password")

**Response:**
- `200 OK` - Returns root CA certificate in PKCS#12 format
- `404 Not Found` - No CA found

**Headers:**
```
Content-Type: application/x-pkcs12
Content-Disposition: attachment; filename="CertA_Root_CA.pfx"
```

## 📊 Data Models

### Certificate Types

```csharp
public enum CertificateType
{
    Server = 0,        // Web server certificates
    Client = 1,        // Client authentication certificates
    CodeSigning = 2,   // Code signing certificates
    Email = 3,         // Email encryption/signing certificates
    Wildcard = 4       // Wildcard certificates for subdomain coverage
}
```

**Certificate Type Descriptions:**
- **Server**: For web servers and HTTPS connections
- **Client**: For client authentication
- **Code Signing**: For software and code signing
- **Email**: For email encryption and signing
- **Wildcard**: For subdomain coverage (e.g., `*.example.com`)

### Certificate Status
```csharp
public enum CertificateStatus
{
    Pending = 0,   // Certificate creation in progress
    Issued = 1,    // Certificate is valid and issued
    Revoked = 2,   // Certificate has been revoked
    Expired = 3    // Certificate has expired
}
```

### User Model
```csharp
public class ApplicationUser
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Organization { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}
```

### Certificate Model
```csharp
public class CertificateEntity
{
    public int Id { get; set; }
    public string CommonName { get; set; }
    public string? SubjectAlternativeNames { get; set; }
    public string SerialNumber { get; set; }
    public DateTime IssuedDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public CertificateStatus Status { get; set; }
    public CertificateType Type { get; set; }
    public string CertificatePem { get; set; }
    public string PublicKeyPem { get; set; }
    public string PrivateKeyPem { get; set; }
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
```

## 🔒 Security

### Authentication Requirements
- **User Registration**: Email must be unique
- **Password Requirements**: Minimum 6 characters with uppercase, lowercase, and numeric
- **Session Management**: 12-hour sessions with sliding expiration
- **User Isolation**: Users can only access their own certificates

### Authorization
- **Certificate Access**: Users can only view/download their own certificates
- **CA Access**: Public access to CA information (no authentication required)
- **Profile Access**: Users can only access their own profile

### Data Protection
- **Private Keys**: Stored encrypted in database
- **Password Hashing**: ASP.NET Core Identity password hashing
- **Session Security**: Secure cookie configuration
- **CSRF Protection**: Anti-forgery token validation

## 📝 HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Request successful |
| 302 | Found - Redirect (authentication, success) |
| 400 | Bad Request - Validation errors |
| 401 | Unauthorized - Authentication required |
| 403 | Forbidden - Access denied |
| 404 | Not Found - Resource not found |
| 500 | Internal Server Error - Server error |

## 🛠️ Examples

### cURL Examples

#### Register New User
```bash
curl -X POST http://localhost:8080/Account/Register \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "FirstName=John&LastName=Doe&Email=john@example.com&Organization=Example&Password=Pass123!&ConfirmPassword=Pass123!"
```

#### Login
```bash
curl -X POST http://localhost:8080/Account/Login \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "Email=john@example.com&Password=Pass123!" \
  -c cookies.txt
```

#### Create Certificate (with authentication)
```bash
curl -X POST http://localhost:8080/Certificates/Create \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -b cookies.txt \
  -d "CommonName=example.com&SubjectAlternativeNames=www.example.com&Type=Server"
```

#### Download Certificate
```bash
curl -X GET http://localhost:8080/Certificates/DownloadCertificate/1 \
  -b cookies.txt \
  -o certificate.pem
```

#### Download Root CA (no authentication required)
```bash
curl -X GET http://localhost:8080/Certificates/DownloadRootCA \
  -o root_ca.pem
```

### PowerShell Examples

#### Register New User
```powershell
$body = @{
    FirstName = "John"
    LastName = "Doe"
    Email = "john@example.com"
    Organization = "Example"
    Password = "Pass123!"
    ConfirmPassword = "Pass123!"
}

Invoke-RestMethod -Uri "http://localhost:8080/Account/Register" -Method POST -Body $body -ContentType "application/x-www-form-urlencoded"
```

#### Login and Create Certificate
```powershell
# Login
$loginBody = @{
    Email = "john@example.com"
    Password = "Pass123!"
}

$session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
$response = Invoke-RestMethod -Uri "http://localhost:8080/Account/Login" -Method POST -Body $loginBody -ContentType "application/x-www-form-urlencoded" -WebSession $session

# Create certificate
$certBody = @{
    CommonName = "example.com"
    SubjectAlternativeNames = "www.example.com"
    Type = "Server"
}

Invoke-RestMethod -Uri "http://localhost:8080/Certificates/Create" -Method POST -Body $certBody -ContentType "application/x-www-form-urlencoded" -WebSession $session
```

## 🔍 Error Handling

### Validation Errors
When form validation fails, the response includes error messages:

```html
<div class="text-danger field-validation-error" data-valmsg-for="Email">
    The Email field is required.
</div>
```

### Authentication Errors
- **Invalid credentials**: "Invalid login attempt."
- **Account not found**: Redirects to login page
- **Access denied**: Redirects to login page

### Certificate Errors
- **Certificate not found**: 404 Not Found
- **Access denied**: 302 Found (redirect to login)
- **Invalid parameters**: 400 Bad Request

## 📚 Related Documentation

- **[User Guide](USER_GUIDE.md)** - End-user documentation
- **[Deployment Guide](DEPLOYMENT.md)** - Production deployment
- **[Architecture Guide](ARCHITECTURE.md)** - Technical architecture
- **[Documentation Index](INDEX.md)** - Complete documentation overview

---

*Last updated: August 2025*
