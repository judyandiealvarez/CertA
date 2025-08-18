# CertA User Guide

Complete guide for using the CertA Certification Authority system.

## 🚀 Getting Started

### Prerequisites
- Modern web browser (Chrome, Firefox, Safari, Edge)
- Access to the CertA web interface
- Docker (if running locally)

### First Time Setup

1. **Access the Application**
   - Open your browser and navigate to the CertA URL
   - You'll be redirected to the login page

2. **Default Admin Account**
   - **Email**: `admin@certa.local`
   - **Password**: `Admin123!`
   - **Note**: Change this password immediately after first login

3. **Install Root CA Certificate**
   - Navigate to "Certificate Authority" (no login required)
   - Download the Root CA Certificate (PEM format)
   - Install it in your system's trusted certificate store

## 👤 User Account Management

### Creating a New Account

1. **Access Registration**
   - Click "Register" in the navigation bar
   - Or navigate to `/Account/Register`

2. **Fill in Your Details**
   - **First Name**: Your first name
   - **Last Name**: Your last name
   - **Email**: Your email address (used for login)
   - **Organization**: Your company or organization (optional)
   - **Password**: Create a secure password
   - **Confirm Password**: Re-enter your password

3. **Password Requirements**
   - Minimum 6 characters
   - Must contain uppercase letters
   - Must contain lowercase letters
   - Must contain numeric characters

4. **Complete Registration**
   - Click "Create Account"
   - You'll be automatically logged in and redirected to the dashboard

### Logging In

1. **Access Login Page**
   - Click "Login" in the navigation bar
   - Or navigate to `/Account/Login`

2. **Enter Credentials**
   - **Email**: Your registered email address
   - **Password**: Your account password
   - **Remember Me**: Check to stay logged in (optional)

3. **Authentication**
   - Click "Login"
   - You'll be redirected to the dashboard or your intended destination

### Managing Your Profile

1. **Access Profile**
   - Click on your username in the navigation bar
   - Select "Profile" from the dropdown menu

2. **Update Personal Information**
   - **First Name**: Update your first name
   - **Last Name**: Update your last name
   - **Organization**: Update your organization
   - **Email**: Displayed but cannot be changed
   - **Account Created**: Displayed but cannot be changed
   - **Account Status**: Shows if your account is active

3. **Save Changes**
   - Click "Update Profile"
   - You'll see a success message confirming the update

### Changing Your Password

1. **Access Password Change**
   - Navigate to your Profile page
   - Scroll down to the "Change Password" section

2. **Enter Password Information**
   - **Current Password**: Your current password
   - **New Password**: Your new password
   - **Confirm New Password**: Re-enter your new password

3. **Password Requirements**
   - Minimum 6 characters
   - Must contain uppercase letters
   - Must contain lowercase letters
   - Must contain numeric characters

4. **Complete Password Change**
   - Click "Change Password"
   - You'll see a success message confirming the change

### Logging Out

1. **Access Logout**
   - Click on your username in the navigation bar
   - Select "Logout" from the dropdown menu

2. **Session Termination**
   - Your session will be terminated
   - You'll be redirected to the home page
   - You'll need to log in again to access protected features

## 📜 Certificate Management

### Understanding Certificates

#### Certificate Types
- **Server**: For web servers, load balancers, and API endpoints
- **Client**: For client authentication and VPN connections
- **Code Signing**: For signing software and applications
- **Email**: For email encryption and digital signatures

#### Certificate Components
- **Common Name (CN)**: Primary domain name (e.g., `example.com`)
- **Subject Alternative Names (SAN)**: Additional domain names or IP addresses
- **Validity Period**: How long the certificate is valid (typically 1 year)
- **Key Pair**: Public key (certificate) and private key

### Creating Certificates

1. **Access Certificate Creation**
   - Login to your account
   - Navigate to "My Certificates"
   - Click "New Certificate"

2. **Fill in Certificate Details**
   - **Common Name**: Primary domain name (e.g., `example.com`)
   - **Subject Alternative Names**: Additional domains (e.g., `www.example.com, api.example.com`)
   - **Certificate Type**: Select appropriate type (Server, Client, Code Signing, Email)

3. **Create Certificate**
   - Click "Create Certificate"
   - The system will generate a CA-signed certificate
   - You'll be redirected to the certificate details page

### Viewing Your Certificates

1. **Access Certificate List**
   - Login to your account
   - Navigate to "My Certificates"
   - View all your certificates in a table format

2. **Certificate Information Displayed**
   - **Common Name**: Primary domain name
   - **Subject Alternative Names**: Additional domains
   - **Serial Number**: Unique certificate identifier
   - **Issued Date**: When the certificate was created
   - **Expiry Date**: When the certificate expires
   - **Status**: Current certificate status (Issued, Expired, Revoked)
   - **Type**: Certificate type (Server, Client, etc.)

3. **Certificate Details**
   - Click on a certificate to view detailed information
   - See full certificate information and download options

### Downloading Certificates

#### Available Formats

1. **Certificate (PEM)**
   - Contains only the public certificate
   - Use for web server configuration
   - File extension: `.pem` or `.crt`

2. **Private Key (PEM)**
   - Contains only the private key
   - Keep secure and never share
   - File extension: `.key` or `.pem`

3. **Public Key (PEM)**
   - Contains only the public key
   - Use for client verification
   - File extension: `.pub` or `.pem`

4. **PFX/PKCS#12**
   - Contains both certificate and private key
   - Password protected
   - Use for Windows servers and applications
   - File extension: `.pfx` or `.p12`

#### Download Process

1. **Access Certificate Details**
   - Click on a certificate from your list
   - Navigate to the "Download Files" section

2. **Choose Download Format**
   - Click the appropriate download button
   - For PFX files, the default password is `password`

3. **Save Files Securely**
   - Save private keys in a secure location
   - Use appropriate file permissions
   - Never share private keys

### Certificate Installation

#### Web Servers

**Apache HTTP Server:**
```apache
SSLCertificateFile /path/to/certificate.pem
SSLCertificateKeyFile /path/to/private_key.pem
SSLCertificateChainFile /path/to/ca_bundle.pem
```

**Nginx:**
```nginx
ssl_certificate /path/to/certificate.pem;
ssl_certificate_key /path/to/private_key.pem;
```

**IIS (Windows):**
1. Import the PFX file
2. Enter the password when prompted
3. Assign to your website

#### Load Balancers

**HAProxy:**
```
ssl-default-bind-ciphers ECDHE-RSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384
ssl-default-bind-options no-sslv3 no-tlsv10 no-tlsv11
bind *:443 ssl crt /path/to/certificate.pem
```

**Nginx (Load Balancer):**
```nginx
upstream backend {
    server backend1.example.com:443 ssl;
    server backend2.example.com:443 ssl;
}
```

## 🏛️ Certificate Authority

### Understanding the CA

The Certificate Authority (CA) is the root of trust for all certificates issued by CertA. Installing the root CA certificate in your system's trusted store makes all CertA-issued certificates trusted.

### Accessing CA Information

1. **Public Access**
   - No login required
   - Navigate to "Certificate Authority" in the main menu
   - View CA information and download options

2. **CA Information Displayed**
   - **Name**: CA name and organization
   - **Common Name**: CA certificate common name
   - **Organization**: CA organization details
   - **Location**: Country, state, and locality
   - **Created Date**: When the CA was created
   - **Expiry Date**: When the CA expires (typically 10 years)
   - **Status**: Whether the CA is active

### Installing the Root CA

#### Windows

1. **Download CA Certificate**
   - Click "Root CA Certificate (PEM)" on the CA page
   - Save the file to your computer

2. **Install Certificate**
   - Right-click the downloaded `.pem` file
   - Select "Install Certificate"
   - Choose "Local Machine"
   - Select "Trusted Root Certification Authorities"
   - Click "Next" and "Finish"

3. **Verify Installation**
   - Open Command Prompt as Administrator
   - Run: `certmgr.msc`
   - Check "Trusted Root Certification Authorities" → "Certificates"

#### macOS

1. **Download CA Certificate**
   - Click "Root CA Certificate (PEM)" on the CA page
   - Save the file to your computer

2. **Install Certificate**
   - Double-click the downloaded `.pem` file
   - Keychain Access will open
   - Select "System" keychain
   - Click "Add"

3. **Set Trust Settings**
   - Find the CA certificate in Keychain Access
   - Double-click to open
   - Expand "Trust" section
   - Set "When using this certificate" to "Always Trust"
   - Close and save changes

#### Linux

1. **Download CA Certificate**
   - Click "Root CA Certificate (PEM)" on the CA page
   - Save the file to your computer

2. **Install Certificate**
   ```bash
   # Copy to system certificates directory
   sudo cp CertA_Root_CA.pem /usr/local/share/ca-certificates/
   
   # Update certificate store
   sudo update-ca-certificates
   ```

3. **Verify Installation**
   ```bash
   # Check if certificate is installed
   openssl verify -CAfile /usr/local/share/ca-certificates/CertA_Root_CA.pem your_certificate.pem
   ```

### Using PFX Format

For Windows systems or applications that require PFX format:

1. **Download PFX**
   - Click "Root CA Certificate (PFX)" on the CA page
   - Default password is `password`

2. **Import PFX**
   - Use Windows Certificate Manager
   - Or import directly into applications that support PFX

## 🔒 Security Best Practices

### Password Security
- Use strong, unique passwords
- Change passwords regularly
- Never share passwords
- Use password managers for secure storage

### Certificate Security
- Keep private keys secure and encrypted
- Use appropriate file permissions (600 for private keys)
- Never share private keys
- Regularly rotate certificates
- Monitor certificate expiration dates

### System Security
- Use HTTPS in production environments
- Keep systems updated
- Implement proper access controls
- Monitor for suspicious activity
- Regular security audits

### CA Security
- Protect the root CA private key
- Limit access to CA management
- Regular CA backups
- Monitor CA certificate expiration

## 🐛 Troubleshooting

### Common Issues

#### Authentication Problems
**Problem**: Can't log in
**Solutions**:
- Verify email and password are correct
- Check if account is active
- Try password reset (if available)
- Contact administrator

**Problem**: Session expires frequently
**Solutions**:
- Check "Remember Me" option
- Verify system time is correct
- Clear browser cookies and cache

#### Certificate Issues
**Problem**: Certificate not trusted
**Solutions**:
- Install root CA certificate
- Verify certificate chain
- Check certificate expiration
- Verify certificate purpose matches usage

**Problem**: PFX import fails
**Solutions**:
- Verify password is correct (default: `password`)
- Check if certificate is properly formatted
- Try different import method
- Verify certificate is not corrupted

**Problem**: Certificate doesn't work with specific application
**Solutions**:
- Check certificate type matches application requirements
- Verify Subject Alternative Names include required domains
- Check application-specific configuration
- Verify certificate format compatibility

#### Browser Issues
**Problem**: Browser shows security warnings
**Solutions**:
- Install root CA certificate
- Clear browser cache and cookies
- Check browser security settings
- Verify certificate is valid

### Getting Help

1. **Check Documentation**
   - Review this user guide
   - Check API documentation
   - Review deployment guides

2. **Check Logs**
   - Review application logs
   - Check system logs
   - Look for error messages

3. **Contact Support**
   - Report issues with detailed information
   - Include error messages and logs
   - Provide steps to reproduce the problem

## 📚 Additional Resources

### Documentation
- **[API Documentation](API.md)** - Complete API reference
- **[Deployment Guide](DEPLOYMENT.md)** - Production deployment
- **[Architecture Guide](ARCHITECTURE.md)** - Technical details

### External Resources
- **X.509 Certificate Standards**: RFC 5280
- **ACME Protocol**: RFC 8555
- **PKCS#12 Standard**: RFC 7292
- **OpenSSL Documentation**: https://www.openssl.org/docs/

### Best Practices
- **OWASP Security Guidelines**: https://owasp.org/
- **NIST Cybersecurity Framework**: https://www.nist.gov/cyberframework
- **CIS Controls**: https://www.cisecurity.org/controls/

---

**Happy certificate management!**

*Last updated: August 2025*
