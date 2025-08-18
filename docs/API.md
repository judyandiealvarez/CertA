# CertA API Documentation

## Overview

The CertA API provides RESTful endpoints for managing certificates and the certificate authority. All endpoints return JSON responses and use standard HTTP status codes.

## Base URL

```
http://localhost:8080
```

## Authentication

Currently, the API does not require authentication. For production use, implement proper authentication and authorization.

## Response Format

All API responses follow this format:

```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}
```

## Error Responses

```json
{
  "success": false,
  "error": "Error message",
  "details": "Additional error details"
}
```

## HTTP Status Codes

- `200` - Success
- `201` - Created
- `400` - Bad Request
- `404` - Not Found
- `500` - Internal Server Error

---

## Certificate Management

### List Certificates

**GET** `/Certificates`

Returns a list of all certificates.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "commonName": "example.com",
      "subjectAlternativeNames": "www.example.com,api.example.com",
      "serialNumber": "1EEAA3E178CCC9CCEA312D1CA801BD8DE4315A62",
      "issuedDate": "2025-08-18T13:59:52Z",
      "expiryDate": "2026-08-18T13:59:52Z",
      "status": "Issued",
      "type": "Server"
    }
  ]
}
```

### Get Certificate Details

**GET** `/Certificates/{id}`

Returns detailed information about a specific certificate.

**Parameters:**
- `id` (integer) - Certificate ID

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "commonName": "example.com",
    "subjectAlternativeNames": "www.example.com,api.example.com",
    "serialNumber": "1EEAA3E178CCC9CCEA312D1CA801BD8DE4315A62",
    "issuedDate": "2025-08-18T13:59:52Z",
    "expiryDate": "2026-08-18T13:59:52Z",
    "status": "Issued",
    "type": "Server",
    "certificatePem": "-----BEGIN CERTIFICATE-----\n...",
    "publicKeyPem": "-----BEGIN PUBLIC KEY-----\n...",
    "privateKeyPem": "-----BEGIN RSA PRIVATE KEY-----\n..."
  }
}
```

### Create Certificate

**POST** `/Certificates/Create`

Creates a new certificate signed by the CA.

**Request Body:**
```json
{
  "commonName": "example.com",
  "subjectAlternativeNames": "www.example.com,api.example.com",
  "type": "Server"
}
```

**Parameters:**
- `commonName` (string, required) - Primary domain name
- `subjectAlternativeNames` (string, optional) - Comma-separated list of additional domains
- `type` (enum, required) - Certificate type: `Server`, `Client`

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 2,
    "commonName": "example.com",
    "status": "Issued"
  },
  "message": "Certificate created successfully"
}
```

### Download Certificate (PEM)

**GET** `/Certificates/DownloadCertificate/{id}`

Downloads the certificate in PEM format.

**Parameters:**
- `id` (integer) - Certificate ID

**Response:** Binary file download

### Download Private Key (PEM)

**GET** `/Certificates/DownloadPrivateKey/{id}`

Downloads the private key in PEM format.

**Parameters:**
- `id` (integer) - Certificate ID

**Response:** Binary file download

### Download Public Key (PEM)

**GET** `/Certificates/DownloadPublicKey/{id}`

Downloads the public key in PEM format.

**Parameters:**
- `id` (integer) - Certificate ID

**Response:** Binary file download

### Download Certificate (PFX)

**GET** `/Certificates/DownloadPfx/{id}`

Downloads the certificate and private key in PKCS#12 format.

**Parameters:**
- `id` (integer) - Certificate ID
- `password` (string, optional) - PFX password (default: "password")

**Response:** Binary file download

---

## Certificate Authority Management

### Get CA Information

**GET** `/Certificates/Authority`

Returns information about the root CA.

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "CertA Root CA",
    "commonName": "CertA Root CA",
    "organization": "CertA Organization",
    "country": "US",
    "state": "California",
    "locality": "San Francisco",
    "createdDate": "2025-08-18T13:59:52Z",
    "expiryDate": "2035-08-18T13:59:52Z",
    "isActive": true,
    "certificatePem": "-----BEGIN CERTIFICATE-----\n..."
  }
}
```

### Download Root CA Certificate (PEM)

**GET** `/Certificates/DownloadRootCA`

Downloads the root CA certificate in PEM format.

**Response:** Binary file download

### Download Root CA Certificate (PFX)

**GET** `/Certificates/DownloadRootCAPfx`

Downloads the root CA certificate and private key in PKCS#12 format.

**Parameters:**
- `password` (string, optional) - PFX password (default: "password")

**Response:** Binary file download

---

## Certificate Types

### Server Certificate
- **Purpose**: Web servers, load balancers, API endpoints
- **Key Usage**: Digital Signature, Key Encipherment
- **Extended Key Usage**: Server Authentication
- **Validity**: 1 year

### Client Certificate
- **Purpose**: Client authentication, VPN connections
- **Key Usage**: Digital Signature, Key Encipherment
- **Extended Key Usage**: Client Authentication
- **Validity**: 1 year

---

## Certificate Status

- **Issued**: Certificate is valid and active
- **Expired**: Certificate has passed its expiry date
- **Revoked**: Certificate has been revoked (future feature)

---

## Error Codes

| Code | Description |
|------|-------------|
| `CERT_NOT_FOUND` | Certificate with specified ID not found |
| `CA_NOT_FOUND` | No active certificate authority found |
| `INVALID_REQUEST` | Request parameters are invalid |
| `CERT_CREATION_FAILED` | Failed to create certificate |
| `DOWNLOAD_FAILED` | Failed to generate download file |

---

## Rate Limiting

Currently, no rate limiting is implemented. For production use, implement appropriate rate limiting to prevent abuse.

## Security Considerations

1. **HTTPS**: Always use HTTPS in production
2. **Authentication**: Implement proper authentication
3. **Authorization**: Restrict access based on user roles
4. **Input Validation**: Validate all input parameters
5. **Logging**: Log all certificate operations for audit

---

## Examples

### cURL Examples

**Create a certificate:**
```bash
curl -X POST http://localhost:8080/Certificates/Create \
  -H "Content-Type: application/json" \
  -d '{
    "commonName": "api.example.com",
    "subjectAlternativeNames": "api.example.com,api2.example.com",
    "type": "Server"
  }'
```

**Download certificate:**
```bash
curl -O http://localhost:8080/Certificates/DownloadCertificate/1
```

**Download root CA:**
```bash
curl -O http://localhost:8080/Certificates/DownloadRootCA
```

### PowerShell Examples

**Create a certificate:**
```powershell
$body = @{
    commonName = "example.com"
    subjectAlternativeNames = "www.example.com,api.example.com"
    type = "Server"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:8080/Certificates/Create" `
  -Method POST `
  -Body $body `
  -ContentType "application/json"
```

**Download certificate:**
```powershell
Invoke-WebRequest -Uri "http://localhost:8080/Certificates/DownloadCertificate/1" `
  -OutFile "certificate.pem"
```
