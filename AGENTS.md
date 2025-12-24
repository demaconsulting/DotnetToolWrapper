# GitHub Copilot Agents Configuration

This repository contains configuration and instructions for GitHub Copilot agents to assist with development tasks.

## Repository Overview

**DotnetToolWrapper** is a .NET 8.0, 9.0, and 10.0 console application that serves as a wrapper for native
applications packaged as [.NET Tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools).

## Project Structure

```text
DotnetToolWrapper/
├── .github/
│   └── workflows/           # GitHub Actions workflows
│       ├── build.yaml       # Reusable build workflow
│       ├── build_on_push.yaml # Triggered on push events
│       └── release.yaml     # Release workflow
├── src/
│   └── DemaConsulting.DotnetToolWrapper/
│       ├── Program.cs       # Main application logic
│       └── DemaConsulting.DotnetToolWrapper.csproj
├── .cspell.json            # Spelling check configuration
├── .markdownlint.json      # Markdown linting configuration
├── README.md               # Project documentation
├── LICENSE                 # MIT License
└── spdx-workflow.yaml      # SBOM enhancement workflow
```

## Key Technologies

- **.NET 8.0, 9.0, 10.0**: Multi-targeted framework versions
- **C# 12**: Programming language
- **GitHub Actions**: CI/CD automation
- **SBOM Tools**: Software Bill of Materials generation

## Development Guidelines

### Building the Project

```bash
dotnet restore
dotnet build --configuration Release
```

### Code Standards

- **Language Version**: C# 12
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled
- **Target Frameworks**: net8.0, net9.0, net10.0

### Workflow Structure

The project uses a modular workflow approach:

1. **build_on_push.yaml**: Triggers on every push, calls the reusable build workflow
2. **build.yaml**: Reusable workflow that performs the actual build, SBOM generation, and artifact upload
3. **release.yaml**: Handles release-specific tasks

### Quality Checks

Quality checks are automated through GitHub Actions:

- **Spelling**: Checked using `cspell` against `.cspell.json` configuration
- **Markdown Linting**: Validated using `markdownlint-cli` against `.markdownlint.json` configuration

## Agent Instructions

### When Working with Code

1. **Multi-targeting**: Always ensure changes are compatible with .NET 8.0, 9.0, and 10.0
2. **Cross-platform**: The tool must work on Windows, Linux, FreeBSD, and macOS
3. **Architecture Support**: Support x86, x64, ARM, ARM64, WASM, and S390x architectures

### When Modifying Workflows

1. Preserve the reusable workflow pattern
2. Update both `build.yaml` and `build_on_push.yaml` if needed
3. Ensure SBOM generation continues to work

### When Updating Documentation

1. Follow the markdown linting rules in `.markdownlint.json`
2. Check spelling against `.cspell.json` dictionary
3. Keep README.md synchronized with actual functionality
4. Update ARCHITECTURE.md if making architectural changes
5. Reference appropriate documentation files (README.md, ARCHITECTURE.md, CONTRIBUTING.md, SECURITY.md)

### When Addressing Security Issues

1. Follow the security policy outlined in `SECURITY.md`
2. Never commit sensitive information
3. Consider all supported platforms when evaluating security impact
4. Update security documentation if introducing new security considerations

## Common Tasks

### Adding New Dependencies

Update the `.csproj` file in `src/DemaConsulting.DotnetToolWrapper/`

### Modifying Build Output

Edit the "Create Drop Folder" step in `.github/workflows/build.yaml`

### Updating Supported Frameworks

1. Modify `<TargetFrameworks>` in the `.csproj` file
2. Update workflow files to include new framework versions
3. Update the "Create Drop Folder" step to copy artifacts for new frameworks

## Testing Changes

Before committing:

1. Build locally: `dotnet build --configuration Release`
2. Run spelling checks: `npx cspell "**/*.md"`
3. Run markdown linting: `npx markdownlint "**/*.md"`

## Related Documentation

- [README.md](README.md) - Project overview and quick start guide
- [ARCHITECTURE.md](ARCHITECTURE.md) - Detailed architecture and design documentation
- [CONTRIBUTING.md](CONTRIBUTING.md) - Contribution guidelines and development setup
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) - Community code of conduct
- [SECURITY.md](SECURITY.md) - Security policy and vulnerability reporting
- [LICENSE](LICENSE) - MIT License terms

## Contact

For questions or issues, please refer to the repository's issue tracker on GitHub.
