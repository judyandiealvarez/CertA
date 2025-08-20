# CertA - Certification Authority

[![License: LGPL v3](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](https://www.gnu.org/licenses/lgpl-3.0)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
[![CI/CD](https://github.com/judyandiealvarez/CertA/workflows/CI%2FCD%20Pipeline/badge.svg)](https://github.com/judyandiealvarez/CertA/actions)
[![Docker](https://img.shields.io/docker/pulls/judyandiealvarez/certa.svg)](https://hub.docker.com/r/judyandiealvarez/certa)
[![Security](https://github.com/judyandiealvarez/CertA/workflows/Security%20Scan/badge.svg)](https://github.com/judyandiealvarez/CertA/actions)

A comprehensive Certification Authority (CA) system with web-based certificate management, built with ASP.NET Core and PostgreSQL.

## 🌟 Features

### 🔐 Authentication & Authorization
- **User Registration & Login** - Secure user account management
- **User Isolation** - Each user can only see and manage their own certificates
- **Profile Management** - Users can update their information and change passwords
- **Session Management** - Configurable cookie-based authentication with 12-hour sessions

### 🏛️ Certificate Authority
- **Root CA Management** - Self-signed root certificate authority
- **Certificate Issuance** - CA-signed certificates with proper X.509 extensions
- **Certificate Types** - Server, Client, Code Signing, and Email certificates
- **Subject Alternative Names (SAN)** - Support for multiple domain names and IP addresses

### 📜 Certificate Management
- **Web-based Interface** - User-friendly certificate creation and management
- **Multiple Formats** - Download certificates in PEM, PFX/PKCS#12 formats
- **Key Management** - Separate downloads for public and private keys
- **Certificate Details** - Comprehensive certificate information display

### 🔧 Technical Features
- **ACME Protocol Support** - Automated certificate management (planned)
- **Database Storage** - PostgreSQL with Entity Framework Core
- **Docker Support** - Complete containerization with Docker Compose
- **Cross-platform** - Works on Windows, macOS, and Linux

## 🚀 Quick Start

### Prerequisites
- Docker and Docker Compose
- Modern web browser

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CertA
   ```

2. **Start the application**
   ```bash
   docker-compose up -d
   ```

3. **Access the application**
   - Open your browser to `http://localhost:8080`
   - You'll be redirected to the login page

4. **Default Admin Account**
   - **Email**: `admin@certa.local`
   - **Password**: `Admin123!`

## 👤 User Management

### Registration
1. Click "Register" in the navigation bar
2. Fill in your details (First Name, Last Name, Email, Organization)
3. Create a secure password (minimum 6 characters with uppercase, lowercase, and numeric)
4. Click "Create Account"

### Login
1. Enter your email and password
2. Optionally check "Remember me" for persistent sessions
3. Click "Login"

### Profile Management
1. Click on your username in the navigation bar
2. Select "Profile" from the dropdown
3. Update your personal information
4. Change your password if needed
5. Click "Update Profile" or "Change Password"

## 📋 Certificate Operations

### Creating Certificates
1. **Login** to your account
2. **Navigate** to "My Certificates"
3. **Click** "New Certificate"
4. **Fill in** the certificate details:
   - Common Name (e.g., `example.com`)
   - Subject Alternative Names (optional, comma-separated)
   - Certificate Type (Server, Client, Code Signing, Email)
5. **Click** "Create Certificate"

### Downloading Certificates
1. **View** your certificate details
2. **Download** in your preferred format:
   - **Certificate (PEM)** - Public certificate only
   - **Private Key (PEM)** - Private key only
   - **Public Key (PEM)** - Public key only
   - **PFX/PKCS#12** - Certificate with private key (password protected)

### Certificate Authority
- **Public Access** - Anyone can view CA information without login
- **Root CA Download** - Download the root CA certificate for trust establishment
- **Installation Instructions** - Platform-specific guides for CA installation

## 🏗️ Architecture

### System Components
- **Web Interface** - ASP.NET Core MVC application
- **Database** - PostgreSQL with Entity Framework Core
- **Authentication** - ASP.NET Core Identity
- **Certificate Services** - Custom services for CA and certificate management
- **Containerization** - Docker and Docker Compose

### Security Model
- **User Isolation** - Complete separation of user data
- **Authentication Required** - All sensitive operations require login
- **Password Security** - Enforced password requirements
- **Session Management** - Secure cookie-based sessions
- **Data Protection** - Encrypted data storage

## 🔧 Configuration

### Environment Variables
```bash
# Database
ConnectionStrings__DefaultConnection=Host=postgres;Database=certa;Username=certa;Password=certa123;Port=5432

# Application
ASPNETCORE_ENVIRONMENT=Development
```

### Docker Configuration
- **PostgreSQL**: Port 5433 (host) → 5432 (container)
- **Web Application**: Port 8080 (host) → 8080 (container)
- **Volumes**: Persistent data storage for database and logs

## 📚 Documentation

- **[API Documentation](docs/API.md)** - Complete REST API reference
- **[Deployment Guide](docs/DEPLOYMENT.md)** - Production deployment instructions
- **[Architecture Guide](docs/ARCHITECTURE.md)** - Technical architecture details
- **[User Guide](docs/USER_GUIDE.md)** - End-user documentation
- **[Documentation Index](docs/INDEX.md)** - Complete documentation overview

## 🔒 Security Considerations

### Authentication
- **Password Requirements**: Minimum 6 characters with complexity
- **Session Security**: Configurable session timeouts
- **Account Lockout**: Protection against brute force attacks
- **Secure Cookies**: HTTP-only, secure cookie configuration

### Certificate Security
- **Private Key Protection**: Secure storage and download
- **CA Security**: Root CA private key protection
- **Certificate Validation**: Proper X.509 extension validation
- **User Isolation**: Complete data separation between users

### Data Protection
- **Database Security**: Encrypted connections and access control
- **File System Security**: Secure storage of certificates and keys
- **Network Security**: HTTPS enforcement in production

## 🛠️ Development

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL
- Docker (optional)

### Local Development
1. **Clone** the repository
2. **Configure** database connection
3. **Run** `dotnet restore` and `dotnet build`
4. **Apply** database migrations: `dotnet ef database update`
5. **Start** the application: `dotnet run`

### Database Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 🐳 Docker Deployment

### Production Deployment
```bash
# Build and start
docker-compose -f docker-compose.prod.yml up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Environment Configuration
```bash
# Copy environment file
cp .env.example .env

# Edit configuration
nano .env

# Start with environment
docker-compose --env-file .env up -d
```

## 📊 Monitoring & Logging

### Application Logs
- **Serilog Integration** - Structured logging
- **File Logging** - Persistent log storage
- **Console Logging** - Development debugging
- **Log Levels** - Configurable verbosity

### Health Checks
- **Database Connectivity** - PostgreSQL health monitoring
- **Application Status** - Service availability checks
- **Certificate Status** - CA and certificate validation

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch
3. **Make** your changes
4. **Test** thoroughly
5. **Submit** a pull request

### Development Guidelines
- **Code Style** - Follow C# coding conventions
- **Testing** - Include unit and integration tests
- **Documentation** - Update relevant documentation
- **Security** - Follow security best practices

## 📄 License

This project is licensed under the GNU Lesser General Public License v3.0 - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

### Getting Help
- **Documentation** - Check the [docs](docs/) directory
- **Issues** - Report bugs and feature requests
- **Discussions** - Ask questions and share ideas

### Common Issues
- **Database Connection** - Check PostgreSQL service and connection string
- **Authentication** - Verify user credentials and account status
- **Certificate Issues** - Check CA installation and trust settings
- **Docker Problems** - Ensure Docker services are running

## 🔄 CI/CD Pipeline

### Automated Workflows
- **Build & Test** - Automatic compilation and testing on every push
- **Docker Images** - Multi-platform Docker builds for amd64, arm64, and arm/v7
- **Security Scanning** - Vulnerability scanning with Trivy and CodeQL
- **Dependency Updates** - Automated package updates with PR creation
- **Release Management** - Automatic GitHub releases with changelog generation

### Docker Hub Integration
- **Repository**: `judyandiealvarez/certa`
- **Tags**: `latest`, `main`, version tags (v1.0.0, 1.0, 1)
- **Multi-Platform**: Support for Intel, AMD, and ARM architectures
- **Security**: Automated vulnerability scanning and reporting

### Quick Deployment
```bash
# Latest stable version
docker run -d -p 8080:8080 judyandiealvarez/certa:latest

# Specific version
docker run -d -p 8080:8080 judyandiealvarez/certa:v1.0.0

# Development version
docker run -d -p 8080:8080 judyandiealvarez/certa:main
```

For detailed workflow information, see [.github/workflows/README.md](.github/workflows/README.md).

## 🗺️ Roadmap

### Planned Features
- **ACME Protocol** - Full RFC 8555 implementation
- **Certificate Revocation** - CRL and OCSP support
- **Intermediate CAs** - Multi-level CA hierarchy
- **API Authentication** - JWT token-based API access
- **Certificate Monitoring** - Expiration alerts and renewal
- **Web Server Integration** - Apache, Nginx, IIS integration
- **High Availability** - Load balancing and clustering
- **Advanced Security** - Hardware security modules (HSM) support

### Version History
- **v1.0.0** - Initial release with basic CA functionality
- **v1.1.0** - Added user authentication and authorization
- **v1.2.0** - Profile management and password changes
- **v1.3.0** - Enhanced security and monitoring (planned)

---

**CertA** - Secure Certificate Authority Management Made Simple

*Last updated: August 2025*
test
# Trigger new build
