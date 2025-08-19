# Contributing to CertA

Thank you for your interest in contributing to CertA! This document provides guidelines and information for contributors.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Code Standards](#code-standards)
- [Pull Request Process](#pull-request-process)
- [Reporting Bugs](#reporting-bugs)
- [Feature Requests](#feature-requests)
- [Security Issues](#security-issues)

## Code of Conduct

This project and everyone participating in it is governed by our Code of Conduct. By participating, you are expected to uphold this code.

### Our Standards

- Use welcoming and inclusive language
- Be respectful of differing viewpoints and experiences
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

## How Can I Contribute?

### Reporting Bugs

- Use the GitHub issue tracker
- Include detailed steps to reproduce the bug
- Provide environment information (OS, .NET version, etc.)
- Include error messages and stack traces
- Describe the expected behavior

### Suggesting Enhancements

- Use the GitHub issue tracker
- Describe the enhancement clearly
- Explain why this enhancement would be useful
- Include mockups or examples if applicable

### Pull Requests

- Fork the repository
- Create a feature branch (`git checkout -b feature/amazing-feature`)
- Make your changes
- Add tests if applicable
- Ensure all tests pass
- Update documentation
- Submit a pull request

## Development Setup

### Prerequisites

- .NET 8.0 SDK or later
- Docker and Docker Compose
- PostgreSQL (or use Docker)
- Git

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/judyandiealvarez/CertA.git
   cd CertA
   ```

2. **Set up the database**
   ```bash
   docker-compose up -d postgres
   ```

3. **Configure the application**
   - Copy `appsettings.Development.json.example` to `appsettings.Development.json`
   - Update connection strings and settings as needed

4. **Run the application**
   ```bash
   cd CertA
   dotnet run
   ```

5. **Run tests**
   ```bash
   dotnet test
   ```

### Docker Development

```bash
# Build and run with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f certa-app

# Stop services
docker-compose down
```

## Code Standards

### C# Coding Standards

- Follow Microsoft C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public APIs
- Keep methods focused and under 50 lines when possible
- Use async/await for I/O operations
- Handle exceptions appropriately

### Code Style

```csharp
// Good
public async Task<CertificateEntity> CreateCertificateAsync(
    string commonName, 
    string organization, 
    string userId)
{
    if (string.IsNullOrEmpty(commonName))
        throw new ArgumentException("Common name cannot be null or empty", nameof(commonName));
    
    // Implementation
}

// Avoid
public async Task<CertificateEntity> CreateCert(string cn, string org, string uid)
{
    // Implementation without validation
}
```

### File Organization

- Keep related classes in the same namespace
- Use appropriate folder structure
- Separate concerns (Controllers, Services, Models, etc.)
- Keep files under 500 lines when possible

### Database

- Use Entity Framework Core migrations
- Follow naming conventions for tables and columns
- Add appropriate indexes
- Use transactions for multi-step operations

## Pull Request Process

### Before Submitting

1. **Ensure your code compiles**
   ```bash
   dotnet build
   ```

2. **Run tests**
   ```bash
   dotnet test
   ```

3. **Check code style**
   ```bash
   dotnet format
   ```

4. **Update documentation**
   - Update README.md if needed
   - Add XML documentation for new APIs
   - Update API documentation

### Pull Request Guidelines

1. **Title**: Use a clear, descriptive title
   - Good: "Add support for ECDSA certificates"
   - Bad: "Fix stuff"

2. **Description**: Include:
   - What changes were made
   - Why the changes were made
   - How to test the changes
   - Any breaking changes

3. **Tests**: Include tests for new functionality

4. **Documentation**: Update relevant documentation

### Review Process

1. **Automated Checks**: Ensure all CI/CD checks pass
2. **Code Review**: At least one maintainer must approve
3. **Testing**: Verify functionality works as expected
4. **Merge**: Once approved, maintainers will merge

## Reporting Bugs

### Bug Report Template

```markdown
**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Steps to reproduce the behavior:
1. Go to '...'
2. Click on '....'
3. Scroll down to '....'
4. See error

**Expected behavior**
A clear and concise description of what you expected to happen.

**Screenshots**
If applicable, add screenshots to help explain your problem.

**Environment:**
 - OS: [e.g. macOS, Windows, Linux]
 - .NET Version: [e.g. 8.0.0]
 - Browser: [e.g. Chrome, Safari]
 - Version: [e.g. 22]

**Additional context**
Add any other context about the problem here.
```

## Feature Requests

### Feature Request Template

```markdown
**Is your feature request related to a problem? Please describe.**
A clear and concise description of what the problem is.

**Describe the solution you'd like**
A clear and concise description of what you want to happen.

**Describe alternatives you've considered**
A clear and concise description of any alternative solutions or features you've considered.

**Additional context**
Add any other context or screenshots about the feature request here.
```

## Security Issues

### Reporting Security Vulnerabilities

**Do not report security vulnerabilities through public GitHub issues.**

Instead, please report them via email to: [security@certa.local]

You should receive a response within 48 hours. If for some reason you do not, please follow up via email to ensure we received your original message.

Please include the requested information listed below (as much as you can provide) to help us better understand the nature and scope of the possible issue:

- Type of issue (buffer overflow, SQL injection, cross-site scripting, etc.)
- Full paths of source file(s) related to the vulnerability
- The location of the affected source code (tag/branch/commit or direct URL)
- Any special configuration required to reproduce the issue
- Step-by-step instructions to reproduce the issue
- Proof-of-concept or exploit code (if possible)
- Impact of the issue, including how an attacker might exploit it

This information will help us triage your report more quickly.

## Getting Help

If you need help with contributing:

- Check existing issues and pull requests
- Join our discussions in GitHub Discussions
- Review the documentation in the `/docs` folder
- Check the API documentation

## License

By contributing to CertA, you agree that your contributions will be licensed under the LGPL-3.0 License.

## Recognition

Contributors will be recognized in:
- The project README
- Release notes
- GitHub contributors page

Thank you for contributing to CertA!
