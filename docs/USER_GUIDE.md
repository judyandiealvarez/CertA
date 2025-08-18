# CertA User Guide

## Introduction

Welcome to CertA, your self-hosted Certification Authority! This guide will help you understand how to use CertA to create and manage certificates for your infrastructure.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Understanding Certificates](#understanding-certificates)
3. [Certificate Authority Management](#certificate-authority-management)
4. [Creating Certificates](#creating-certificates)
5. [Managing Certificates](#managing-certificates)
6. [Installing Certificates](#installing-certificates)
7. [Troubleshooting](#troubleshooting)
8. [Best Practices](#best-practices)

---

## Getting Started

### First Time Setup

1. **Access the Web Interface**
   - Open your web browser
   - Navigate to `http://your-certa-server:8080`
   - You should see the CertA dashboard

2. **Install the Root CA Certificate**
   - Click on **"Certificate Authority"** in the navigation
   - Download the **Root CA Certificate (PEM)**
   - Install it as a trusted authority (see installation instructions below)

3. **Create Your First Certificate**
   - Click **"Create New Certificate"** from the dashboard
   - Fill in the required information
   - Download and install the certificate

### Understanding the Interface

The CertA web interface consists of several main areas:

- **Dashboard**: Overview of your certificates and quick actions
- **Certificates**: List and manage all your certificates
- **Certificate Authority**: View and download the root CA certificate
- **Create Certificate**: Form to create new certificates

---

## Understanding Certificates

### What is a Certificate Authority (CA)?

A Certificate Authority is a trusted entity that issues digital certificates. Think of it like a digital notary that vouches for the identity of websites, servers, or applications.

### Certificate Types

#### Server Certificates
- **Purpose**: Secure web servers, APIs, and services
- **Use Cases**: HTTPS websites, API endpoints, load balancers
- **Validity**: 1 year
- **Key Usage**: Digital signatures and encryption

#### Client Certificates
- **Purpose**: Authenticate clients to servers
- **Use Cases**: VPN connections, client authentication
- **Validity**: 1 year
- **Key Usage**: Digital signatures and encryption

### Certificate Components

A certificate contains:
- **Common Name (CN)**: Primary domain name (e.g., `example.com`)
- **Subject Alternative Names (SAN)**: Additional domain names
- **Public Key**: Used for encryption and verification
- **Private Key**: Used for decryption and signing (keep secure!)
- **Validity Period**: When the certificate is valid
- **Issuer**: The CA that signed the certificate

### Trust Chain

```
Your Certificate
    ↓ (signed by)
CertA Root CA
    ↓ (trusted by)
Your System
```

When you install the CertA Root CA as trusted, all certificates issued by CertA become trusted by your system.

---

## Certificate Authority Management

### Viewing CA Information

1. Navigate to **Certificate Authority** in the menu
2. View the CA details:
   - **Name**: CertA Root CA
   - **Organization**: CertA Organization
   - **Validity Period**: 10 years
   - **Status**: Active

### Installing the Root CA Certificate

#### Windows

1. **Download the Root CA Certificate**
   - Go to Certificate Authority page
   - Click **"Root CA Certificate (PEM)"**

2. **Install the Certificate**
   - Double-click the downloaded `.pem` file
   - Click **"Install Certificate"**
   - Choose **"Local Machine"**
   - Click **"Next"**
   - Select **"Place all certificates in the following store"**
   - Click **"Browse"**
   - Select **"Trusted Root Certification Authorities"**
   - Click **"OK"** and **"Next"**
   - Click **"Finish"**

3. **Verify Installation**
   - Open **Certificate Manager** (certmgr.msc)
   - Navigate to **Trusted Root Certification Authorities > Certificates**
   - Look for **"CertA Root CA"**

#### macOS

1. **Download the Root CA Certificate**
   - Go to Certificate Authority page
   - Click **"Root CA Certificate (PEM)"**

2. **Install the Certificate**
   - Double-click the downloaded `.pem` file
   - Keychain Access will open
   - Select **"System"** keychain
   - Click **"Add"**

3. **Set Trust Settings**
   - Find **"CertA Root CA"** in the System keychain
   - Double-click to open
   - Expand **"Trust"**
   - Set **"When using this certificate"** to **"Always Trust"**
   - Close the window

4. **Verify Installation**
   - Open **Keychain Access**
   - Search for **"CertA Root CA"**
   - Verify it shows as trusted

#### Linux

1. **Download the Root CA Certificate**
   ```bash
   wget http://your-certa-server:8080/Certificates/DownloadRootCA -O CertA_Root_CA.pem
   ```

2. **Install the Certificate**
   ```bash
   sudo cp CertA_Root_CA.pem /usr/local/share/ca-certificates/
   sudo update-ca-certificates
   ```

3. **Verify Installation**
   ```bash
   openssl verify -CAfile /usr/local/share/ca-certificates/CertA_Root_CA.pem /path/to/your/certificate.pem
   ```

---

## Creating Certificates

### Step-by-Step Process

1. **Access the Creation Form**
   - Click **"Create New Certificate"** from the dashboard
   - Or navigate to **Certificates > Create**

2. **Fill in Certificate Details**

   **Common Name (Required)**
   - Enter the primary domain name
   - Example: `example.com`
   - This is the main domain the certificate will secure

   **Subject Alternative Names (Optional)**
   - Enter additional domain names, separated by commas
   - Example: `www.example.com, api.example.com, mail.example.com`
   - These domains will also be secured by the certificate

   **Certificate Type**
   - **Server**: For web servers, APIs, and services
   - **Client**: For client authentication

3. **Create the Certificate**
   - Click **"Create Certificate"**
   - The system will generate the certificate
   - You'll be redirected to the certificate details page

### Certificate Details

After creation, you'll see:
- **Certificate Information**: Common name, SANs, validity period
- **Download Options**: Certificate, private key, public key, and PFX
- **Certificate Content**: Full PEM content for verification

---

## Managing Certificates

### Viewing Certificates

1. **Certificate List**
   - Navigate to **Certificates** in the menu
   - View all certificates in a table format
   - See status, expiry date, and type

2. **Certificate Details**
   - Click **"View"** on any certificate
   - See complete certificate information
   - Access download options

### Certificate Status

- **Issued**: Certificate is valid and active
- **Expired**: Certificate has passed its expiry date
- **Revoked**: Certificate has been revoked (future feature)

### Downloading Certificates

#### Certificate (PEM)
- **Use**: For web servers (Apache, Nginx)
- **Format**: Base64 encoded text
- **Contains**: Certificate only

#### Private Key (PEM)
- **Use**: For web servers (Apache, Nginx)
- **Format**: Base64 encoded text
- **Contains**: Private key only
- **Security**: Keep this file secure!

#### Public Key (PEM)
- **Use**: For verification and distribution
- **Format**: Base64 encoded text
- **Contains**: Public key only
- **Security**: Safe to share

#### PFX/PKCS#12
- **Use**: For Windows servers (IIS)
- **Format**: Binary file
- **Contains**: Certificate and private key
- **Password**: Default is "password"

---

## Installing Certificates

### Apache Web Server

1. **Download Certificate Files**
   - Download **Certificate (PEM)**
   - Download **Private Key (PEM)**

2. **Configure Apache**
   ```apache
   <VirtualHost *:443>
       ServerName example.com
       DocumentRoot /var/www/html
       
       SSLEngine on
       SSLCertificateFile /path/to/certificate.pem
       SSLCertificateKeyFile /path/to/private_key.pem
       
       # Optional: Include CA certificate
       SSLCertificateChainFile /path/to/ca_certificate.pem
   </VirtualHost>
   ```

3. **Restart Apache**
   ```bash
   sudo systemctl restart apache2
   ```

### Nginx Web Server

1. **Download Certificate Files**
   - Download **Certificate (PEM)**
   - Download **Private Key (PEM)**

2. **Configure Nginx**
   ```nginx
   server {
       listen 443 ssl;
       server_name example.com;
       
       ssl_certificate /path/to/certificate.pem;
       ssl_certificate_key /path/to/private_key.pem;
       
       # Optional: Include CA certificate
       ssl_trusted_certificate /path/to/ca_certificate.pem;
       
       location / {
           root /var/www/html;
           index index.html;
       }
   }
   ```

3. **Restart Nginx**
   ```bash
   sudo systemctl restart nginx
   ```

### Windows IIS

1. **Download PFX File**
   - Download **PFX/PKCS#12** file
   - Note the password (default: "password")

2. **Import Certificate**
   - Open **IIS Manager**
   - Select your server
   - Double-click **"Server Certificates"**
   - Click **"Import"** in the Actions pane
   - Browse to the PFX file
   - Enter the password
   - Click **"OK"**

3. **Bind Certificate**
   - Select your website
   - Click **"Bindings"** in the Actions pane
   - Click **"Add"**
   - Choose **"https"** and select your certificate
   - Click **"OK"**

### Load Balancers

#### HAProxy
```haproxy
frontend https_frontend
    bind *:443 ssl crt /path/to/certificate.pem
    default_backend web_servers

backend web_servers
    server web1 10.0.1.10:80 check
    server web2 10.0.1.11:80 check
```

#### AWS Application Load Balancer
1. Upload certificate to AWS Certificate Manager
2. Create HTTPS listener
3. Select your certificate
4. Configure target group

---

## Troubleshooting

### Common Issues

#### Certificate Not Trusted
**Problem**: Browser shows "Not Secure" or certificate warning

**Solutions**:
1. Ensure root CA is installed as trusted
2. Check certificate chain
3. Verify certificate is for the correct domain

**Verification**:
```bash
# Check certificate chain
openssl verify -CAfile ca_certificate.pem your_certificate.pem

# View certificate details
openssl x509 -in certificate.pem -text -noout
```

#### PFX Import Fails
**Problem**: Windows can't import PFX file

**Solutions**:
1. Verify password is correct (default: "password")
2. Check file integrity
3. Ensure you have import permissions

#### Web Server Won't Start
**Problem**: Apache/Nginx fails to start after certificate installation

**Solutions**:
1. Check file permissions
2. Verify certificate and key match
3. Check configuration syntax

**Debugging**:
```bash
# Apache
sudo apache2ctl configtest

# Nginx
sudo nginx -t
```

#### Certificate Expired
**Problem**: Certificate has expired

**Solutions**:
1. Create a new certificate
2. Install the new certificate
3. Restart services

### Certificate Validation

#### Check Certificate Validity
```bash
# View certificate details
openssl x509 -in certificate.pem -text -noout

# Check expiry date
openssl x509 -in certificate.pem -noout -dates

# Verify certificate chain
openssl verify -CAfile ca_certificate.pem certificate.pem
```

#### Test HTTPS Connection
```bash
# Test with curl
curl -I https://example.com

# Test with openssl
openssl s_client -connect example.com:443 -servername example.com
```

### Log Analysis

#### Application Logs
Check CertA application logs for errors:
```bash
# Docker
docker-compose logs certa-app

# System service
sudo journalctl -u certa -f
```

#### Web Server Logs
```bash
# Apache
sudo tail -f /var/log/apache2/error.log

# Nginx
sudo tail -f /var/log/nginx/error.log
```

---

## Best Practices

### Security

1. **Private Key Protection**
   - Never share private keys
   - Use appropriate file permissions (600)
   - Store securely with encryption
   - Rotate keys regularly

2. **Certificate Management**
   - Monitor expiry dates
   - Renew certificates before expiration
   - Use appropriate key sizes (2048-bit minimum)
   - Implement certificate revocation procedures

3. **Network Security**
   - Use HTTPS for all web traffic
   - Implement HSTS headers
   - Use secure cipher suites
   - Regular security updates

### Certificate Planning

1. **Domain Strategy**
   - Plan your domain structure
   - Use wildcard certificates sparingly
   - Group related domains in single certificates
   - Document certificate inventory

2. **Renewal Process**
   - Set up automated renewal reminders
   - Test renewal procedures
   - Maintain backup certificates
   - Document installation procedures

3. **Monitoring**
   - Monitor certificate expiry
   - Check certificate validity
   - Monitor security events
   - Regular compliance audits

### Operational Procedures

1. **Documentation**
   - Document certificate inventory
   - Record installation procedures
   - Maintain contact information
   - Update procedures regularly

2. **Backup and Recovery**
   - Backup certificates and keys
   - Test recovery procedures
   - Secure backup storage
   - Regular backup testing

3. **Compliance**
   - Follow security policies
   - Maintain audit trails
   - Regular security assessments
   - Compliance reporting

---

## Support

### Getting Help

1. **Check Documentation**
   - Review this user guide
   - Check troubleshooting section
   - Review error messages

2. **System Logs**
   - Check application logs
   - Review web server logs
   - Analyze error messages

3. **Community Support**
   - Create issue in repository
   - Provide detailed error information
   - Include system details

### Contact Information

For technical support:
- Create an issue in the project repository
- Include detailed error messages
- Provide system configuration details
- Attach relevant log files

---

## Conclusion

CertA provides a powerful and flexible solution for managing your own certificate infrastructure. By following this guide, you can effectively create, manage, and deploy certificates for your organization's needs.

Remember to:
- Keep your root CA secure
- Monitor certificate expiry dates
- Follow security best practices
- Maintain proper documentation

Happy certificate management!
