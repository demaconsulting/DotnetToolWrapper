# GitHub Copilot Agents Configuration

This repository contains configuration and instructions for GitHub Copilot agents to assist with development tasks.

## Repository Overview

**DotnetToolWrapper** is a .NET 8.0, 9.0, and 10.0 console application that serves as a wrapper for native
applications packaged as [.NET Tools][dotnet-tools].

## Project Structure

```text
DotnetToolWrapper/
├── .config/
│   └── dotnet-tools.json    # .NET tools configuration (sbom-tool, spdx-tool)
├── .github/
│   ├── ISSUE_TEMPLATE/      # GitHub issue templates
│   │   ├── bug_report.yml
│   │   ├── feature_request.yml
│   │   └── config.yml
│   ├── dependabot.yml       # Dependabot configuration
│   └── workflows/           # GitHub Actions workflows
│       ├── build.yaml       # Reusable build workflow
│       ├── build_on_push.yaml # Triggered on push events
│       └── release.yaml     # Release workflow
├── docs/
│   └── usage.md             # Detailed usage documentation
├── src/
│   └── DemaConsulting.DotnetToolWrapper/
│       ├── Program.cs       # Main application logic
│       └── DemaConsulting.DotnetToolWrapper.csproj
├── test/
│   └── DemaConsulting.DotnetToolWrapper.Tests/
│       ├── IntegrationTests.cs
│       ├── ProgramTests.cs
│       ├── Runner.cs
│       └── DemaConsulting.DotnetToolWrapper.Tests.csproj
├── .cspell.json            # Spelling check configuration
├── .editorconfig           # Editor configuration for consistent formatting
├── .gitignore              # Git ignore patterns
├── .markdownlint.json      # Markdown linting configuration
├── AGENTS.md               # This file - Agent instructions
├── ARCHITECTURE.md         # Architecture documentation
├── CODE_OF_CONDUCT.md      # Community code of conduct
├── CONTRIBUTING.md         # Contribution guidelines
├── DemaConsulting.DotnetToolWrapper.sln
├── LICENSE                 # MIT License
├── README.md               # Project documentation
├── SECURITY.md             # Security policy
└── spdx-workflow.yaml      # SBOM enhancement workflow
```

## Key Technologies

- **.NET 8.0, 9.0, 10.0**: Multi-targeted framework versions
- **C# 12**: Programming language
- **MSTest**: Testing framework
- **GitHub Actions**: CI/CD automation
- **SBOM Tools**: Software Bill of Materials generation (sbom-tool, spdx-tool)
- **Code Analyzers**: Microsoft.CodeAnalysis.NetAnalyzers 10.0.101, SonarAnalyzer.CSharp 10.17.0.131074

## Development Guidelines

### Building the Project

```bash
dotnet restore
dotnet build --configuration Release
```

### Testing the Project

```bash
# Run all tests
dotnet test --configuration Release

# Run tests with detailed output
dotnet test --configuration Release --logger "console;verbosity=detailed"
```

The test project uses MSTest and includes:

- **Unit Tests**: `ProgramTests.cs` - Tests for core program logic
- **Integration Tests**: `IntegrationTests.cs` - End-to-end tests with actual execution
- **Test Runner**: `Runner.cs` - Helper for running the tool in tests

### Code Standards

- **Language Version**: C# 12
- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled
- **Target Frameworks**: net8.0, net9.0, net10.0
- **EditorConfig**: Follow the .editorconfig rules for consistent code formatting

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
4. **Testing**: Always run tests after making changes. The CI runs tests on ubuntu-latest,
   windows-latest, and macos-latest
5. **Code Quality**: Maintain `TreatWarningsAsErrors` - all warnings must be fixed
6. **Analyzers**: Keep analyzer packages (Microsoft.CodeAnalysis.NetAnalyzers and
   SonarAnalyzer.CSharp) at the same version across all projects

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

1. Update the `.csproj` file in `src/DemaConsulting.DotnetToolWrapper/`
2. If adding analyzer packages, ensure test project also gets the same version
3. Dependabot is configured to automatically update NuGet packages in the `nuget-dependencies` group

### Modifying Build Output

Edit the "Create Drop Folder" step in `.github/workflows/build.yaml`

### Updating Supported Frameworks

1. Modify `<TargetFrameworks>` in the `.csproj` file
2. Update workflow files to include new framework versions
3. Update the "Create Drop Folder" step to copy artifacts for new frameworks

## Testing Changes

Before committing:

1. Build locally: `dotnet build --configuration Release`
2. Run tests: `dotnet test --configuration Release`
3. Run spelling checks: `npx cspell "**/*.md"`
4. Run markdown linting: `npx markdownlint "**/*.md"`

## Issue Templates

The repository includes structured issue templates:

- **Bug Report** (`.github/ISSUE_TEMPLATE/bug_report.yml`) - For reporting bugs with system information
- **Feature Request** (`.github/ISSUE_TEMPLATE/feature_request.yml`) - For suggesting new features
- **Config** (`.github/ISSUE_TEMPLATE/config.yml`) - Links to discussions and other resources

## Related Documentation

- [README.md](README.md) - Project overview and quick start guide
- [ARCHITECTURE.md](ARCHITECTURE.md) - Detailed architecture and design documentation
- [CONTRIBUTING.md](CONTRIBUTING.md) - Contribution guidelines and development setup
- [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) - Community code of conduct
- [SECURITY.md](SECURITY.md) - Security policy and vulnerability reporting
- [docs/usage.md](docs/usage.md) - Comprehensive usage guide with examples
- [LICENSE](LICENSE) - MIT License terms

## Contact

For questions or issues, please refer to the repository's issue tracker on GitHub.

[dotnet-tools]: https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools
