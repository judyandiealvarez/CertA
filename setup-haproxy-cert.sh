#!/bin/bash

# Script to download wildcard certificate from CertA and set up HAProxy
# Run this script after you have generated a wildcard certificate in CertA

echo "=== HAProxy Certificate Setup ==="
echo "This script will help you download your wildcard certificate from CertA"
echo "and set it up for HAProxy on the balancer server."
echo ""

# Check if we're on the balancer server
if [ "$(hostname)" = "balancer" ]; then
    echo "✅ Running on balancer server (10.11.2.3)"
else
    echo "❌ This script should be run on the balancer server (10.11.2.3)"
    exit 1
fi

# Create certificate directory
echo "📁 Creating certificate directory..."
sudo mkdir -p /etc/ssl/certs
sudo chown haproxy:haproxy /etc/ssl/certs

echo ""
echo "=== Certificate Download Instructions ==="
echo "1. Go to your CertA application: http://10.11.2.6:8080"
echo "2. Login to your account"
echo "3. Navigate to 'My Certificates'"
echo "4. Find your wildcard certificate (e.g., *.example.com)"
echo "5. Download the certificate in PEM format"
echo "6. Download the private key in PEM format"
echo ""

echo "=== Certificate Preparation ==="
echo "After downloading, you need to combine the certificate and private key:"
echo ""
echo "1. Create the combined certificate file:"
echo "   sudo nano /etc/ssl/certs/certa-wildcard.pem"
echo ""
echo "2. Add the private key first, then the certificate:"
echo "   -----BEGIN PRIVATE KEY-----"
echo "   [Your private key content]"
echo "   -----END PRIVATE KEY-----"
echo "   -----BEGIN CERTIFICATE-----"
echo "   [Your certificate content]"
echo "   -----END CERTIFICATE-----"
echo ""
echo "3. Set proper permissions:"
echo "   sudo chown haproxy:haproxy /etc/ssl/certs/certa-wildcard.pem"
echo "   sudo chmod 600 /etc/ssl/certs/certa-wildcard.pem"
echo ""

echo "=== HAProxy Configuration ==="
echo "The HAProxy configuration is already set up at:"
echo "/etc/haproxy/haproxy.cfg"
echo ""
echo "To test the configuration:"
echo "sudo haproxy -f /etc/haproxy/haproxy.cfg -c"
echo ""
echo "To start HAProxy:"
echo "sudo systemctl start haproxy"
echo "sudo systemctl enable haproxy"
echo ""
echo "To check HAProxy status:"
echo "sudo systemctl status haproxy"
echo ""

echo "=== Access Information ==="
echo "Once HAProxy is running:"
echo "- HTTP: http://10.11.2.3 (redirects to HTTPS)"
echo "- HTTPS: https://10.11.2.3"
echo "- Stats: http://10.11.2.3:8404/stats (admin/admin123)"
echo ""

echo "=== Backend Servers ==="
echo "HAProxy will load balance to:"
echo "- 10.11.2.8:8080 (docker2)"
echo "- 10.11.2.9:8080 (docker3)"
echo ""

echo "Ready to proceed with certificate setup!"
