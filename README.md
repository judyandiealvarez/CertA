# CertA - Certification Authority

A complete, self-hosted Certification Authority (CA) built with ASP.NET Core that allows you to create and manage your own trusted certificates for internal infrastructure.

## 🚀 Features

### Core CA Functionality
- **Root CA Management**: Create and manage your own Certificate Authority
- **CA-Signed Certificates**: Issue certificates signed by your root CA (not self-signed)
- **Trust Chain**: Install root CA to make all issued certificates trusted
- **Multiple Certificate Types**: Support for Server, Client, and other certificate types
- **Subject Alternative Names (SAN)**: Support for multiple domain names per certificate

### Certificate Management
- **Web UI**: Complete web interface for certificate management
- **Multiple Download Formats**: 
  - Certificate (PEM)
  - Private Key (PEM)
  - Public Key (PEM)
  - PKCS#12/PFX (with password protection)
- **Certificate Details**: View full certificate information and validity
- **Certificate Lifecycle**: Create, view, download, and manage certificates

### Security & Compliance
- **Proper CA Hierarchy**: Root CA → Issued Certificates
- **Strong Cryptography**: RSA 2048-bit keys for certificates, 4096-bit for CA
- **X.509 Standards**: Full compliance with X.509 certificate standards
- **Database Storage**: Secure storage of certificates and keys in PostgreSQL

## 🏗️ Architecture

```
CertA Certification Authority
├── Root CA Certificate (4096-bit RSA)
│   ├── Self-signed with proper CA extensions
│   ├── 10-year validity period
│   └── Downloadable in PEM/PFX formats
└── Issued Certificates (2048-bit RSA)
    ├── Signed by Root CA
    ├── 1-year validity period
    ├── Support for SAN extensions
    └── Multiple download formats
```

## 📋 Prerequisites

- Docker and Docker Compose
- Modern web browser
- PostgreSQL (included in Docker setup)

## 🛠️ Installation

### Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CertA
   ```

2. **Start the application**
   ```bash
   docker-compose up -d
   ```

3. **Access the web interface**
   ```
   http://localhost:8080
   ```

### Manual Installation

1. **Install .NET 9.0 SDK**
2. **Install PostgreSQL**
3. **Configure connection string** in `appsettings.json`
4. **Run the application**
   ```bash
   dotnet run
   ```

## 🎯 Getting Started

### 1. Install Root CA Certificate

1. Navigate to **Certificate Authority** in the web interface
2. Download the **Root CA Certificate (PEM)**
3. Install it as a trusted authority in your system:

   **macOS:**
   ```bash
   # Double-click the .pem file
   # Add to System keychain
   # Set trust to "Always Trust"
   ```

   **Windows:**
   ```bash
   # Right-click and "Install Certificate"
   # Choose "Local Machine"
   # Select "Trusted Root Certification Authorities"
   ```

   **Linux:**
   ```bash
   sudo cp CertA_Root_CA.pem /usr/local/share/ca-certificates/
   sudo update-ca-certificates
   ```

### 2. Create Your First Certificate

1. Click **"Create New Certificate"** in the web interface
2. Fill in the certificate details:
   - **Common Name**: Your domain (e.g., `example.com`)
   - **Subject Alternative Names**: Additional domains (e.g., `www.example.com, api.example.com`)
   - **Certificate Type**: Server (for web servers)
3. Click **"Create Certificate"**

### 3. Download and Install Certificate

1. View the certificate details
2. Download the certificate files:
   - **Certificate (PEM)**: For web servers
   - **Private Key (PEM)**: Keep secure
   - **PFX/PKCS#12**: For Windows servers
3. Install on your web server

## 🔧 Configuration

### Environment Variables

```bash
# Database
POSTGRES_DB=certa
POSTGRES_USER=certa
POSTGRES_PASSWORD=your_secure_password

# Application
ASPNETCORE_ENVIRONMENT=Production
```

### Custom CA Settings

Edit `CertificateService.cs` to customize:
- CA organization details
- Certificate validity periods
- Key sizes
- Certificate extensions

## 📁 Project Structure

```
CertA/
├── Controllers/           # Web API controllers
│   ├── CertificatesController.cs
│   └── HomeController.cs
├── Models/               # Data models
│   ├── CertificateEntity.cs
│   ├── CertificateAuthority.cs
│   └── ErrorViewModel.cs
├── Services/             # Business logic
│   ├── CertificateService.cs
│   └── CertificateAuthorityService.cs
├── Views/                # Web UI views
│   ├── Certificates/
│   ├── Home/
│   └── Shared/
├── Data/                 # Database context
│   └── AppDbContext.cs
└── wwwroot/             # Static assets
```

## 🔒 Security Considerations

### Certificate Security
- **Private Keys**: Never share private keys
- **Root CA**: Keep root CA private key secure
- **Access Control**: Restrict access to the web interface
- **Network Security**: Use HTTPS in production

### Production Deployment
- **HTTPS**: Configure SSL/TLS for the web interface
- **Authentication**: Add user authentication
- **Backup**: Regular database backups
- **Monitoring**: Monitor certificate expiration

## 🚀 API Endpoints

### Certificate Management
- `GET /Certificates` - List all certificates
- `GET /Certificates/Create` - Certificate creation form
- `POST /Certificates/Create` - Create new certificate
- `GET /Certificates/{id}` - View certificate details
- `GET /Certificates/DownloadCertificate/{id}` - Download certificate (PEM)
- `GET /Certificates/DownloadPrivateKey/{id}` - Download private key (PEM)
- `GET /Certificates/DownloadPublicKey/{id}` - Download public key (PEM)
- `GET /Certificates/DownloadPfx/{id}` - Download certificate (PFX)

### CA Management
- `GET /Certificates/Authority` - View CA information
- `GET /Certificates/DownloadRootCA` - Download root CA (PEM)
- `GET /Certificates/DownloadRootCAPfx` - Download root CA (PFX)

## 🐛 Troubleshooting

### Common Issues

**Certificate not trusted:**
- Ensure root CA is installed as trusted authority
- Check certificate chain in browser developer tools

**PFX import fails:**
- Verify password is correct (default: `password`)
- Check if certificate is properly formatted

**Database connection issues:**
- Verify PostgreSQL is running
- Check connection string in `appsettings.json`

### Logs
```bash
# View application logs
docker-compose logs certa-app

# View database logs
docker-compose logs certa-postgres
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For issues and questions:
1. Check the troubleshooting section
2. Review the logs
3. Create an issue in the repository

## 🔄 Roadmap

- [ ] ACME protocol support (Let's Encrypt compatible)
- [ ] Certificate revocation lists (CRL)
- [ ] OCSP responder
- [ ] Intermediate CA support
- [ ] Certificate templates
- [ ] API authentication
- [ ] Certificate monitoring and alerts
- [ ] Integration with popular web servers

---

**CertA** - Your trusted Certification Authority for internal infrastructure.
