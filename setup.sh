#!/bin/bash

# CertA Setup Script
# This script helps set up the CertA certification authority

set -e

echo "🔐 CertA Certification Authority Setup"
echo "======================================"

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

echo "✅ Docker and Docker Compose are available"

# Create necessary directories
echo "📁 Creating directories..."
mkdir -p logs
mkdir -p ca-keys

# Set proper permissions for CA keys directory
chmod 700 ca-keys

echo "✅ Directories created"

# Start the services
echo "🚀 Starting CertA services..."
docker-compose up -d

echo "⏳ Waiting for services to start..."
sleep 10

# Check if PostgreSQL is ready
echo "🔍 Checking database connection..."
until docker-compose exec -T postgres pg_isready -U certa -d certa; do
    echo "⏳ Waiting for PostgreSQL to be ready..."
    sleep 2
done

echo "✅ PostgreSQL is ready"

# Run database migrations
echo "🗄️ Running database migrations..."
docker-compose exec certa-app dotnet ef database update

echo "✅ Database setup complete"

# Check if the application is running
echo "🔍 Checking application status..."
sleep 5

if curl -f http://localhost:5000 > /dev/null 2>&1; then
    echo "✅ CertA is running successfully!"
else
    echo "⚠️ CertA might still be starting up. Please wait a moment and check http://localhost:5000"
fi

echo ""
echo "🎉 Setup Complete!"
echo "=================="
echo ""
echo "📱 Access CertA:"
echo "   Web UI: http://localhost:5000"
echo "   ACME Directory: http://localhost:5000/acme/directory"
echo ""
echo "📋 Next Steps:"
echo "   1. Open http://localhost:5000 in your browser"
echo "   2. Create your first certificate request"
echo "   3. Configure your ACME client to use the ACME directory"
echo ""
echo "🔧 Useful Commands:"
echo "   View logs: docker-compose logs -f"
echo "   Stop services: docker-compose down"
echo "   Restart services: docker-compose restart"
echo ""
echo "📚 Documentation: See README.md for more information"
