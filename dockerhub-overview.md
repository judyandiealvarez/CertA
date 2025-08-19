# CertA - Certification Authority Docker Image

## 🏛️ Overview

CertA is a comprehensive, self-hosted Certification Authority (CA) system built with ASP.NET Core that provides complete certificate management capabilities with user authentication and authorization. This Docker image contains a production-ready CA system that allows you to create and manage your own trusted certificates for internal infrastructure.

## 🌟 Key Features

### 🔐 Authentication & Authorization
- **User Registration & Login** - Secure user account management with ASP.NET Core Identity
- **User Isolation** - Each user can only see and manage their own certificates
- **Profile Management** - Users can update their information and change passwords
- **Session Management** - Configurable cookie-based authentication with 12-hour sessions

### 🏛️ Certificate Authority
- **Root CA Management** - Self-signed root certificate authority with 4096-bit RSA keys
- **CA-Signed Certificates** - Issue certificates signed by your root CA (not self-signed)
- **Trust Chain** - Install root CA to make all issued certificates trusted
- **Multiple Certificate Types** - Server, Client, Code Signing, and Email certificates
- **Subject Alternative Names (SAN)** - Support for multiple domain names and IP addresses

### 📜 Certificate Management
- **Web-based Interface** - User-friendly certificate creation and management
- **Multiple Formats** - Download certificates in PEM, PFX/PKCS#12 formats
- **Key Management** - Separate downloads for public and private keys
- **Certificate Details** - Comprehensive certificate information display

### 🔧 Technical Features
- **ACME Protocol Support** - Automated certificate management (planned)
- **Database Storage** - PostgreSQL with Entity Framework Core
- **Cross-platform** - Works on Windows, macOS, and Linux
- **Production Ready** - Secure configuration and best practices

## 🚀 Quick Start

### Using Docker Compose (Recommended)
```yaml
version: '3.8'
services:
  postgres:
    image: postgres:15-alpine
    environment:
      POSTGRES_DB: certa
      POSTGRES_USER: certa
      POSTGRES_PASSWORD: your_secure_password
    volumes:
      - postgres_data:/var/lib/postgresql/data
    ports:
      - "5433:5432"

  certa-app:
    image: judyandiealvarez/certa:latest
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=certa;Username=certa;Password=your_secure_password;Port=5432
    depends_on:
      - postgres
    volumes:
      - ./logs:/app/logs
      - ./ca-keys:/app/ca-keys

volumes:
  postgres_data:
```

### Using Docker Run
```bash
# Start PostgreSQL
docker run -d --name certa-postgres \
  -e POSTGRES_DB=certa \
  -e POSTGRES_USER=certa \
  -e POSTGRES_PASSWORD=your_secure_password \
  -p 5433:5432 \
  postgres:15-alpine

# Start CertA
docker run -d --name certa-app \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=certa;Username=certa;Password=your_secure_password;Port=5432" \
  judyandiealvarez/certa:latest
```

## 👤 First Time Setup

1. **Access the Application**
   - Open your browser to `http://localhost:8080`
   - You'll be redirected to the login page

2. **Default Admin Account**
   - **Email**: `admin@certa.local`
   - **Password**: `Admin123!`
   - **Note**: Change this password immediately after first login

3. **Install Root CA Certificate**
   - Navigate to "Certificate Authority" (no login required)
   - Download the Root CA Certificate (PEM format)
   - Install it in your system's trusted certificate store

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

## 🔒 Security Features

### Authentication Security
- **Password Requirements**: Minimum 6 characters with complexity
- **Session Security**: Configurable session timeouts
- **Secure Cookies**: HTTP-only, secure cookie configuration
- **CSRF Protection**: Anti-forgery token validation

### Authorization Security
- **User Isolation**: Complete separation of user data
- **Access Control**: Users can only access their own resources
- **Permission Validation**: Server-side validation of all requests
- **Audit Logging**: Comprehensive operation logging

### Certificate Security
- **Private Key Protection**: Encrypted storage in database
- **Secure Downloads**: Protected download endpoints
- **CA Security**: Root CA private key protection
- **Trust Chain**: Proper certificate hierarchy validation

## 🛠️ Configuration

### Environment Variables
```bash
# Database Connection
ConnectionStrings__DefaultConnection=Host=postgres;Database=certa;Username=certa;Password=your_secure_password;Port=5432

# Application Environment
ASPNETCORE_ENVIRONMENT=Production

# Logging
Logging__LogLevel__Default=Information
Logging__LogLevel__Microsoft=Warning
```

### Volume Mounts
- **Logs**: `./logs:/app/logs` - Application logs
- **CA Keys**: `./ca-keys:/app/ca-keys` - Certificate Authority keys (optional)

## 📊 System Requirements

### Minimum Requirements
- **RAM**: 2GB
- **Storage**: 10GB
- **CPU**: 2 cores
- **Network**: Internet access for initial setup

### Recommended Requirements
- **RAM**: 4GB
- **Storage**: 50GB
- **CPU**: 4 cores
- **Network**: Stable internet connection

## 🔧 Troubleshooting

### Common Issues
- **Database Connection**: Verify PostgreSQL is running and accessible
- **Authentication**: Check default admin credentials
- **Certificate Issues**: Ensure root CA is installed as trusted
- **Port Conflicts**: Change port mapping if 8080 is already in use

### Getting Help
- **Documentation**: Check the GitHub repository for detailed guides
- **Issues**: Report problems in the GitHub repository
- **Logs**: Check application logs in the mounted volume

## 📚 Documentation

- **GitHub Repository**: https://github.com/judyandiealvarez/CertA.git
- **User Guide**: Complete end-user documentation
- **API Documentation**: REST API reference
- **Architecture Guide**: Technical implementation details
- **Deployment Guide**: Production deployment instructions

## 🗺️ Roadmap

### Planned Features
- **ACME Protocol**: RFC 8555 implementation for automated certificate management
- **Certificate Revocation**: CRL and OCSP responder support
- **Intermediate CAs**: Multi-level certificate authority hierarchy
- **API Authentication**: JWT token-based API access
- **Certificate Monitoring**: Expiration alerts and renewal automation

### Security Enhancements
- **Multi-Factor Authentication**: MFA support for enhanced security
- **Role-Based Access Control**: Advanced authorization with roles
- **Hardware Security Modules**: HSM integration for key management
- **Certificate Transparency**: CT log integration for monitoring

## 📄 License

This project is licensed under the GNU Lesser General Public License v3.0 - see the [LICENSE](https://github.com/judyandiealvarez/CertA/blob/main/LICENSE) file for details.

## 🤝 Contributing

We welcome contributions! Please see the [Contributing Guide](https://github.com/judyandiealvarez/CertA/blob/main/CONTRIBUTING.md) for details.

## 🆘 Support

- **Documentation**: Check the [docs](https://github.com/judyandiealvarez/CertA/tree/main/docs) directory
- **Issues**: Report bugs and feature requests in the GitHub repository
- **Discussions**: Ask questions and share ideas in GitHub Discussions

---

**CertA** - Secure Certificate Authority Management Made Simple

*Version: v1.2.0 | Last updated: August 2025*
