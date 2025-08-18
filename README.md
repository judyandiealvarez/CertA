# CertA - Certification Authority with ACME Support

CertA is a comprehensive certification authority (CA) built with ASP.NET Core that supports both manual certificate management through a web UI and automated certificate issuance via the ACME protocol.

## Features

### 🔐 Certificate Management
- **Root CA Generation**: Create and manage root certification authorities
- **Certificate Issuance**: Issue certificates for servers, clients, and other purposes
- **Certificate Revocation**: Revoke certificates with configurable reasons
- **Certificate Validation**: Automatic validation of certificate status and expiry
- **Certificate Download**: Download certificates in PEM format

### 🌐 Web UI
- **Dashboard**: Overview of certificates and requests with statistics
- **Certificate Management**: View, create, issue, and revoke certificates
- **Request Management**: Handle certificate requests with approval workflow
- **Modern Interface**: Bootstrap-based responsive design with icons

### 🔄 ACME Protocol Support
- **RFC 8555 Compliance**: Full ACME v2 protocol implementation
- **HTTP-01 Challenges**: Domain validation via HTTP challenges
- **Automated Issuance**: Automated certificate issuance for ACME clients
- **Account Management**: ACME account creation and management

### 🗄️ Database
- **PostgreSQL Support**: Robust database backend with Entity Framework Core
- **Data Persistence**: Store certificates, requests, and ACME data
- **Audit Trail**: Track all certificate operations and changes

## Quick Start

### Prerequisites
- Docker and Docker Compose
- .NET 9.0 SDK (for development)

### Using Docker Compose (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CertA
   ```

2. **Start the services**
   ```bash
   docker-compose up -d
   ```

3. **Access the application**
   - Web UI: http://localhost:5000
   - ACME Directory: http://localhost:5000/acme/directory

4. **Initialize the database** (first time only)
   ```bash
   docker-compose exec certa-app dotnet ef database update
   ```

### Manual Setup

1. **Install dependencies**
   ```bash
   cd CertA
   dotnet restore
   ```

2. **Configure database**
   - Install PostgreSQL
   - Create database and user
   - Update connection string in `appsettings.json`

3. **Run migrations**
   ```bash
   dotnet ef database update
   ```

4. **Start the application**
   ```bash
   dotnet run
   ```

## Usage

### Web UI

1. **Dashboard**: View certificate statistics and recent activity
2. **Certificates**: Manage all issued certificates
3. **Requests**: Handle pending certificate requests
4. **New Certificate**: Create a new certificate request

### ACME Protocol

The ACME endpoints are available at `/acme/` and follow RFC 8555:

- **Directory**: `/acme/directory`
- **New Account**: `POST /acme/new-account`
- **New Order**: `POST /acme/new-order`
- **Authorization**: `GET /acme/authz/{id}`
- **Challenge**: `POST /acme/challenge/{id}`
- **Finalize**: `POST /acme/order/{id}/finalize`
- **Certificate**: `GET /acme/cert/{id}`

### Example ACME Client Usage

Using certbot with CertA:

```bash
# Set ACME server URL
export ACME_SERVER="http://localhost:5000/acme/directory"

# Request certificate
certbot certonly --standalone \
  --server $ACME_SERVER \
  --email your-email@example.com \
  -d example.com \
  -d www.example.com
```

## Configuration

### Environment Variables

- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `CA__PrivateKeyPath`: Path to CA private key file
- `CA__CertificatePath`: Path to CA certificate file

### Docker Environment

The Docker Compose setup includes:
- PostgreSQL 15 database
- CertA application
- Volume mounts for logs and CA keys
- Health checks for database

## Development

### Project Structure

```
CertA/
├── Controllers/          # MVC Controllers
│   ├── HomeController.cs
│   ├── CertificatesController.cs
│   └── AcmeController.cs
├── Models/              # Data Models
│   ├── Certificate.cs
│   ├── CertificateRequest.cs
│   ├── AcmeAccount.cs
│   ├── AcmeOrder.cs
│   ├── AcmeAuthorization.cs
│   └── AcmeChallenge.cs
├── Services/            # Business Logic
│   ├── CertificateAuthorityService.cs
│   └── AcmeService.cs
├── Data/               # Database Context
│   └── CertADbContext.cs
└── Views/              # Razor Views
```

### Database Migrations

```bash
# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

### Running Tests

```bash
dotnet test
```

## Security Considerations

### Production Deployment

1. **HTTPS**: Always use HTTPS in production
2. **Firewall**: Restrict access to necessary ports only
3. **Database Security**: Use strong passwords and network isolation
4. **Key Management**: Secure CA private keys with proper permissions
5. **Backup**: Regular backups of database and CA keys
6. **Monitoring**: Implement logging and monitoring

### CA Key Security

- Store CA private keys securely
- Use hardware security modules (HSM) for production
- Implement key rotation procedures
- Monitor for unauthorized access

## API Documentation

### Certificate Management API

- `GET /Certificates` - List certificates
- `GET /Certificates/{id}` - Get certificate details
- `POST /Certificates/Create` - Create certificate request
- `POST /Certificates/Issue/{id}` - Issue certificate
- `POST /Certificates/Revoke/{id}` - Revoke certificate
- `GET /Certificates/Download/{id}` - Download certificate

### ACME API

All ACME endpoints follow RFC 8555 specification. See the ACME directory at `/acme/directory` for available endpoints.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions:
- Create an issue on GitHub
- Check the documentation
- Review the logs in the `logs/` directory

## Roadmap

- [ ] DNS-01 challenge support
- [ ] Certificate chain validation
- [ ] CRL/OCSP support
- [ ] Multi-CA support
- [ ] API authentication
- [ ] Certificate templates
- [ ] Automated renewal
- [ ] Monitoring and alerting
