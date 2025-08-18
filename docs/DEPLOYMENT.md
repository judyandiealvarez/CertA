# CertA Deployment Guide

## Overview

This guide covers deploying CertA Certification Authority in various environments, from development to production.

## Table of Contents

1. [Development Environment](#development-environment)
2. [Production Deployment](#production-deployment)
3. [Docker Deployment](#docker-deployment)
4. [Kubernetes Deployment](#kubernetes-deployment)
5. [High Availability](#high-availability)
6. [Security Hardening](#security-hardening)
7. [Monitoring and Logging](#monitoring-and-logging)
8. [Backup and Recovery](#backup-and-recovery)

---

## Development Environment

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL 15+
- Visual Studio 2022 or VS Code

### Local Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CertA
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure database**
   ```bash
   # Install PostgreSQL locally or use Docker
   docker run --name postgres-dev -e POSTGRES_PASSWORD=devpass -p 5432:5432 -d postgres:15
   ```

4. **Update connection string**
   ```json
   // appsettings.Development.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=certa;Username=postgres;Password=devpass"
     }
   }
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Access the application**
   ```
   http://localhost:5000
   ```

---

## Production Deployment

### System Requirements

- **CPU**: 2+ cores
- **RAM**: 4GB+ (8GB recommended)
- **Storage**: 50GB+ SSD
- **OS**: Linux (Ubuntu 20.04+, CentOS 8+) or Windows Server 2019+

### Manual Installation

#### 1. Server Preparation

**Ubuntu/Debian:**
```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install .NET 9.0
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-9.0

# Install PostgreSQL
sudo apt install -y postgresql postgresql-contrib
```

**CentOS/RHEL:**
```bash
# Install .NET 9.0
sudo dnf install dotnet-sdk-9.0

# Install PostgreSQL
sudo dnf install postgresql-server postgresql-contrib
sudo postgresql-setup --initdb
sudo systemctl enable postgresql
sudo systemctl start postgresql
```

#### 2. Database Setup

```bash
# Create database and user
sudo -u postgres psql
```

```sql
CREATE DATABASE certa;
CREATE USER certa WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE certa TO certa;
\q
```

#### 3. Application Deployment

```bash
# Create application directory
sudo mkdir -p /opt/certa
sudo chown $USER:$USER /opt/certa

# Copy application files
cp -r CertA/* /opt/certa/

# Create systemd service
sudo tee /etc/systemd/system/certa.service << EOF
[Unit]
Description=CertA Certification Authority
After=network.target postgresql.service

[Service]
Type=exec
User=certa
WorkingDirectory=/opt/certa
ExecStart=/usr/bin/dotnet /opt/certa/CertA.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

# Start service
sudo systemctl daemon-reload
sudo systemctl enable certa
sudo systemctl start certa
```

#### 4. Reverse Proxy (Nginx)

```bash
# Install Nginx
sudo apt install nginx

# Configure Nginx
sudo tee /etc/nginx/sites-available/certa << EOF
server {
    listen 80;
    server_name your-domain.com;
    
    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
        proxy_cache_bypass \$http_upgrade;
    }
}
EOF

# Enable site
sudo ln -s /etc/nginx/sites-available/certa /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

#### 5. SSL/TLS Configuration

```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx

# Obtain SSL certificate
sudo certbot --nginx -d your-domain.com

# Auto-renewal
sudo crontab -e
# Add: 0 12 * * * /usr/bin/certbot renew --quiet
```

---

## Docker Deployment

### Single Container

```bash
# Build and run
docker build -t certa .
docker run -d \
  --name certa \
  -p 8080:8080 \
  -e POSTGRES_DB=certa \
  -e POSTGRES_USER=certa \
  -e POSTGRES_PASSWORD=your_password \
  certa
```

### Docker Compose (Recommended)

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  certa-app:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=certa-postgres;Database=certa;Username=certa;Password=${POSTGRES_PASSWORD}
    depends_on:
      - certa-postgres
    restart: unless-stopped
    volumes:
      - certa-data:/app/data

  certa-postgres:
    image: postgres:15-alpine
    environment:
      - POSTGRES_DB=certa
      - POSTGRES_USER=certa
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
    volumes:
      - postgres-data:/var/lib/postgresql/data
    restart: unless-stopped

volumes:
  certa-data:
  postgres-data:
```

```bash
# Deploy
export POSTGRES_PASSWORD=your_secure_password
docker-compose -f docker-compose.prod.yml up -d
```

---

## Kubernetes Deployment

### Namespace

```yaml
# namespace.yaml
apiVersion: v1
kind: Namespace
metadata:
  name: certa
```

### ConfigMap

```yaml
# configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: certa-config
  namespace: certa
data:
  appsettings.json: |
    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=certa-postgres;Database=certa;Username=certa;Password=${POSTGRES_PASSWORD}"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning"
        }
      }
    }
```

### Secret

```yaml
# secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: certa-secret
  namespace: certa
type: Opaque
data:
  POSTGRES_PASSWORD: <base64-encoded-password>
```

### PostgreSQL Deployment

```yaml
# postgres.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: certa-postgres
  namespace: certa
spec:
  replicas: 1
  selector:
    matchLabels:
      app: certa-postgres
  template:
    metadata:
      labels:
        app: certa-postgres
    spec:
      containers:
      - name: postgres
        image: postgres:15-alpine
        env:
        - name: POSTGRES_DB
          value: "certa"
        - name: POSTGRES_USER
          value: "certa"
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: certa-secret
              key: POSTGRES_PASSWORD
        ports:
        - containerPort: 5432
        volumeMounts:
        - name: postgres-storage
          mountPath: /var/lib/postgresql/data
      volumes:
      - name: postgres-storage
        persistentVolumeClaim:
          claimName: postgres-pvc
---
apiVersion: v1
kind: Service
metadata:
  name: certa-postgres
  namespace: certa
spec:
  selector:
    app: certa-postgres
  ports:
  - port: 5432
    targetPort: 5432
---
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: postgres-pvc
  namespace: certa
spec:
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
```

### Application Deployment

```yaml
# certa.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: certa-app
  namespace: certa
spec:
  replicas: 2
  selector:
    matchLabels:
      app: certa-app
  template:
    metadata:
      labels:
        app: certa-app
    spec:
      containers:
      - name: certa-app
        image: certa:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          value: "Host=certa-postgres;Database=certa;Username=certa;Password=$(POSTGRES_PASSWORD)"
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: certa-secret
              key: POSTGRES_PASSWORD
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: certa-app
  namespace: certa
spec:
  selector:
    app: certa-app
  ports:
  - port: 80
    targetPort: 8080
  type: ClusterIP
---
apiVersion: networking.k8s.io/v1
kind: Ingress
metadata:
  name: certa-ingress
  namespace: certa
  annotations:
    nginx.ingress.kubernetes.io/ssl-redirect: "true"
    cert-manager.io/cluster-issuer: "letsencrypt-prod"
spec:
  tls:
  - hosts:
    - certa.your-domain.com
    secretName: certa-tls
  rules:
  - host: certa.your-domain.com
    http:
      paths:
      - path: /
        pathType: Prefix
        backend:
          service:
            name: certa-app
            port:
              number: 80
```

### Deploy to Kubernetes

```bash
# Apply all resources
kubectl apply -f namespace.yaml
kubectl apply -f secret.yaml
kubectl apply -f configmap.yaml
kubectl apply -f postgres.yaml
kubectl apply -f certa.yaml

# Check status
kubectl get pods -n certa
kubectl get services -n certa
```

---

## High Availability

### Load Balancer Configuration

```yaml
# haproxy.cfg
global
    daemon
    maxconn 4096

defaults
    mode http
    timeout connect 5000ms
    timeout client 50000ms
    timeout server 50000ms

frontend certa-frontend
    bind *:80
    bind *:443 ssl crt /etc/ssl/certs/certa.pem
    redirect scheme https if !{ ssl_fc }
    
    default_backend certa-backend

backend certa-backend
    balance roundrobin
    option httpchk GET /health
    server certa1 10.0.1.10:8080 check
    server certa2 10.0.1.11:8080 check
    server certa3 10.0.1.12:8080 check
```

### Database Clustering

For production, consider using:
- **PostgreSQL with streaming replication**
- **Managed database service** (AWS RDS, Azure Database, GCP Cloud SQL)
- **Database clustering** (Patroni, etcd)

---

## Security Hardening

### Network Security

```bash
# Firewall configuration (UFW)
sudo ufw allow ssh
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw deny 22/tcp
sudo ufw enable
```

### Application Security

```json
// appsettings.Production.json
{
  "Security": {
    "RequireHttps": true,
    "HstsMaxAge": 31536000,
    "ContentSecurityPolicy": "default-src 'self'"
  },
  "Authentication": {
    "Enabled": true,
    "Provider": "JWT"
  }
}
```

### Database Security

```sql
-- PostgreSQL security
ALTER USER certa PASSWORD 'strong_password';
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM PUBLIC;
GRANT CONNECT ON DATABASE certa TO certa;
GRANT USAGE ON SCHEMA public TO certa;
GRANT ALL ON ALL TABLES IN SCHEMA public TO certa;
```

---

## Monitoring and Logging

### Application Monitoring

```yaml
# prometheus.yml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: 'certa'
    static_configs:
      - targets: ['localhost:8080']
    metrics_path: '/metrics'
```

### Logging Configuration

```json
// appsettings.Production.json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "/var/log/certa/certa-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      },
      {
        "Name": "Console"
      }
    ]
  }
}
```

### Health Checks

```csharp
// Add to Program.cs
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));
```

---

## Backup and Recovery

### Database Backup

```bash
#!/bin/bash
# backup.sh
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backup/certa"
mkdir -p $BACKUP_DIR

# Database backup
pg_dump -h localhost -U certa -d certa > $BACKUP_DIR/certa_$DATE.sql

# Compress backup
gzip $BACKUP_DIR/certa_$DATE.sql

# Keep only last 30 days
find $BACKUP_DIR -name "*.sql.gz" -mtime +30 -delete
```

### Certificate Backup

```bash
#!/bin/bash
# cert-backup.sh
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backup/certs"
mkdir -p $BACKUP_DIR

# Backup CA certificates
cp /opt/certa/ca/* $BACKUP_DIR/

# Create archive
tar -czf $BACKUP_DIR/certs_$DATE.tar.gz $BACKUP_DIR/*.pem
```

### Automated Backup

```bash
# Add to crontab
0 2 * * * /opt/certa/scripts/backup.sh
0 3 * * * /opt/certa/scripts/cert-backup.sh
```

### Recovery Procedures

1. **Database Recovery**
   ```bash
   psql -h localhost -U certa -d certa < backup_file.sql
   ```

2. **Application Recovery**
   ```bash
   # Restore from backup
   tar -xzf certs_backup.tar.gz
   cp *.pem /opt/certa/ca/
   
   # Restart application
   sudo systemctl restart certa
   ```

---

## Performance Tuning

### Application Tuning

```json
// appsettings.Production.json
{
  "Kestrel": {
    "Limits": {
      "MaxConcurrentConnections": 100,
      "MaxConcurrentUpgradedConnections": 100,
      "MaxRequestBodySize": 52428800
    }
  }
}
```

### Database Tuning

```sql
-- PostgreSQL tuning
ALTER SYSTEM SET shared_buffers = '256MB';
ALTER SYSTEM SET effective_cache_size = '1GB';
ALTER SYSTEM SET maintenance_work_mem = '64MB';
ALTER SYSTEM SET checkpoint_completion_target = 0.9;
ALTER SYSTEM SET wal_buffers = '16MB';
ALTER SYSTEM SET default_statistics_target = 100;
SELECT pg_reload_conf();
```

---

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   ```bash
   # Check PostgreSQL status
   sudo systemctl status postgresql
   
   # Check connection
   psql -h localhost -U certa -d certa
   ```

2. **Application Startup Issues**
   ```bash
   # Check logs
   sudo journalctl -u certa -f
   
   # Check permissions
   ls -la /opt/certa/
   ```

3. **Certificate Issues**
   ```bash
   # Verify certificate
   openssl x509 -in certificate.pem -text -noout
   
   # Check certificate chain
   openssl verify -CAfile ca.pem certificate.pem
   ```

### Log Analysis

```bash
# Search for errors
grep -i error /var/log/certa/certa-*.log

# Monitor real-time logs
tail -f /var/log/certa/certa-*.log | grep -E "(ERROR|WARN)"
```

---

## Support

For deployment issues:
1. Check the troubleshooting section
2. Review application and system logs
3. Verify network connectivity and firewall rules
4. Create an issue in the repository with detailed error information
