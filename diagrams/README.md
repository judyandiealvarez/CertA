# CertA Draw.io Diagrams

This directory contains comprehensive draw.io diagrams for the CertA Certification Authority system. These diagrams provide visual documentation of the system architecture, processes, and workflows.

## Available Diagrams

### 1. System Architecture (`system-architecture.drawio`)
**Purpose**: High-level overview of the entire CertA system architecture
**Contents**:
- Client layer (web browsers, mobile apps)
- Load balancer (HAProxy)
- Application layer (CertA web app replicas)
- Services layer (Certificate Service, CA Service, Identity Service, Data Protection)
- Data layer (PostgreSQL database, CA keys storage)
- External systems (Docker Hub, GitHub)

**Use Cases**:
- System overview presentations
- Architecture documentation
- Onboarding new team members
- Technical discussions

### 2. CI/CD Pipeline (`ci-cd-pipeline.drawio`)
**Purpose**: Visual representation of the GitHub Actions CI/CD workflow
**Contents**:
- Triggers (push to main/develop, tags, pull requests, manual)
- Test job (checkout, setup .NET, restore, build, test)
- Build job (set tag, Docker login, build, push)
- Deploy jobs (deploy latest, deploy tagged)
- External services (Docker Hub, Docker Swarm)

**Use Cases**:
- DevOps documentation
- Deployment process explanation
- CI/CD troubleshooting
- Process improvement discussions

### 3. Database Schema (`database-schema.drawio`)
**Purpose**: Entity relationship diagram of the database structure
**Contents**:
- AspNetUsers table (Identity system)
- Certificates table (user certificates)
- CertificateAuthorities table (root CA)
- DataProtectionKeys table (ASP.NET Core Data Protection)
- Logs table (Serilog structured logging)
- Relationships and constraints

**Use Cases**:
- Database design documentation
- Data modeling discussions
- Migration planning
- Performance optimization

### 4. Certificate Hierarchy (`certificate-hierarchy.drawio`)
**Purpose**: Visual representation of the certificate authority structure
**Contents**:
- Root CA (self-signed, 4096-bit RSA, 10-year validity)
- Certificate types (Server, Client, Code Signing, Email, Wildcard)
- Certificate extensions (Basic Constraints, Key Usage, Extended Key Usage, etc.)
- Security features (Single CA enforcement, User isolation, Key protection)
- Certificate examples for each type

**Use Cases**:
- Security architecture documentation
- Certificate management explanation
- Compliance documentation
- Training materials

### 5. Deployment Architecture (`deployment-architecture.drawio`)
**Purpose**: Container and deployment infrastructure overview
**Contents**:
- Internet and user access
- HAProxy load balancer
- Docker Swarm cluster (manager and worker nodes)
- Shared storage (PostgreSQL, CA keys, logs)
- External services (Docker Hub, GitHub)
- Network and security configuration
- Monitoring and logging setup

**Use Cases**:
- Infrastructure documentation
- Deployment planning
- High availability discussions
- Disaster recovery planning

### 6. User Flow (`user-flow.drawio`)
**Purpose**: User journey and application workflow
**Contents**:
- User entry points (home page, CA information)
- Authentication flow (login, register, validation)
- Dashboard and navigation
- Certificate management (list, create, details, download)
- Profile management (view, update, change password)
- Certificate creation process
- Download process
- Security features
- Error handling

**Use Cases**:
- User experience documentation
- Feature explanation
- Testing scenarios
- User training materials

## How to Use These Diagrams

### Opening in Draw.io
1. Go to [app.diagrams.net](https://app.diagrams.net) (formerly draw.io)
2. Click "Open Existing Diagram"
3. Select "Device" and choose the `.drawio` file
4. The diagram will load and be ready for editing

### Importing into Draw.io
1. Open draw.io
2. Click "File" → "Import from" → "Device"
3. Select the desired `.drawio` file
4. The diagram will be imported and ready for use

### Customizing Diagrams
- **Colors**: Each diagram uses a consistent color scheme for different component types
- **Shapes**: Standard draw.io shapes are used for compatibility
- **Text**: All text is editable and can be customized for your needs
- **Layout**: Diagrams can be rearranged while maintaining the logical flow

### Exporting Options
- **PNG/JPG**: For presentations and documentation
- **PDF**: For formal documentation
- **SVG**: For scalable graphics
- **XML**: For further editing in draw.io

## Diagram Standards

### Color Coding
- **Blue (#dae8fc)**: Client/User interfaces
- **Green (#d5e8d4)**: Application/Service components
- **Yellow (#fff2cc)**: Load balancers/Infrastructure
- **Red (#f8cecc)**: Security/Critical components
- **Purple (#e1d5e7)**: User management/Profiles
- **Orange (#ffe6cc)**: Data/Storage components
- **Gray (#f5f5f5)**: Configuration/Utility components

### Naming Conventions
- File names use kebab-case (e.g., `system-architecture.drawio`)
- Diagram titles use Title Case
- Component names use descriptive, clear terminology
- Technical terms are consistent across all diagrams

### Layout Principles
- **Top to Bottom**: General flow from user to system
- **Left to Right**: Logical progression of processes
- **Grouping**: Related components are visually grouped
- **Spacing**: Adequate white space for readability
- **Connections**: Clear arrows showing relationships and data flow

## Maintenance

### Updating Diagrams
When making changes to the CertA system:
1. Update the relevant diagram(s) to reflect changes
2. Maintain color coding and naming conventions
3. Test the diagram in draw.io to ensure it loads correctly
4. Update this README if new diagrams are added

### Version Control
- All diagrams are stored in this `diagrams/` directory
- Use descriptive commit messages when updating diagrams
- Consider creating a backup before major changes
- Document significant changes in commit messages

## Integration with Documentation

These diagrams complement the existing documentation:
- **README.md**: High-level system overview
- **docs/ARCHITECTURE.md**: Detailed technical architecture
- **docs/API.md**: API documentation
- **docs/USER_GUIDE.md**: User instructions

The diagrams provide visual context for the written documentation and can be referenced in:
- Technical presentations
- Architecture reviews
- Onboarding materials
- Compliance documentation
- Training sessions

## Contributing

When adding or modifying diagrams:
1. Follow the established color coding and naming conventions
2. Ensure diagrams are self-contained and don't require external resources
3. Test diagrams in draw.io before committing
4. Update this README with information about new diagrams
5. Consider the target audience and use case for each diagram

---

*These diagrams are part of the CertA project documentation and should be kept up-to-date with system changes.*
