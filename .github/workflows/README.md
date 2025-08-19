# GitHub Actions Workflows

This directory contains the CI/CD workflows for the CertA project.

## Workflows Overview

### 🔄 CI/CD Pipeline (`ci.yml`)
**Triggers**: Push to main/develop, Pull Requests, Releases
- **Build & Test**: Compiles the .NET application and runs tests
- **Docker Build**: Creates Docker images for development
- **Release**: Handles official releases with proper tagging

### 🏷️ Create Release (`release.yml`)
**Triggers**: Push tags starting with 'v'
- Automatically creates GitHub releases
- Generates changelog from commits
- Provides Docker image references

### 🔒 Security Scan (`security.yml`)
**Triggers**: Push to main/develop, Pull Requests, Weekly schedule
- Runs Trivy vulnerability scanner
- Performs CodeQL analysis
- Checks .NET package vulnerabilities

### 📦 Update Dependencies (`dependencies.yml`)
**Triggers**: Weekly schedule, Manual dispatch
- Checks for outdated packages
- Creates PRs for dependency updates
- Verifies builds after updates

### 🐳 Docker Multi-Platform Build (`docker-optimize.yml`)
**Triggers**: Push to main, Tags, Manual dispatch
- Builds for multiple architectures (amd64, arm64, arm/v7)
- Optimizes Docker images
- Performs security scanning

## Required Secrets

Configure these secrets in your GitHub repository:

### Docker Hub
- `DOCKERHUB_USERNAME`: Your Docker Hub username
- `DOCKERHUB_TOKEN`: Your Docker Hub access token

### GitHub (Auto-configured)
- `GITHUB_TOKEN`: Automatically provided by GitHub

## Usage

### Creating a Release
1. Create and push a tag:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. The workflow will automatically:
   - Build the application
   - Create Docker images
   - Generate a GitHub release
   - Push images to Docker Hub

### Manual Workflow Dispatch
You can manually trigger workflows from the GitHub Actions tab:
- **Update Dependencies**: Weekly dependency checks
- **Docker Multi-Platform Build**: Build optimized images

## Docker Images

Images are pushed to: `judyandiealvarez/certa`

### Available Tags
- `latest`: Latest stable version
- `main`: Latest from main branch
- `v1.0.0`: Specific version tags
- `1.0`: Major.minor version
- `1`: Major version

### Multi-Platform Support
- `linux/amd64`: Intel/AMD 64-bit
- `linux/arm64`: ARM 64-bit (Apple Silicon, etc.)
- `linux/arm/v7`: ARM 32-bit

## Monitoring

- Check workflow status in the GitHub Actions tab
- Monitor security alerts in the Security tab
- Review dependency updates in Pull Requests
- Track Docker image builds in Docker Hub

## Troubleshooting

### Common Issues
1. **Build Failures**: Check .NET version compatibility
2. **Docker Push Errors**: Verify Docker Hub credentials
3. **Security Scan Failures**: Review vulnerability reports
4. **Dependency Update Conflicts**: Resolve package conflicts manually

### Support
For workflow issues, check:
- GitHub Actions logs
- Docker Hub build logs
- Security scan reports
- Dependency update PRs
