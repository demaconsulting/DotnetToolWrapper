# GitHub Copilot Agents Configuration

This repository contains configuration and instructions for GitHub Copilot agents to assist with development tasks.

## Repository Overview

**DotnetToolWrapper** is a .NET 8.0, 9.0, and 10.0 console application that serves as a wrapper for native
applications packaged as [.NET Tools][dotnet-tools].

## Specialized Agents

This repository includes specialized GitHub Copilot agents to assist with specific aspects of the project:

### Documentation Writer

**Location**: `.github/agents/documentation-writer.md`

An expert technical writer specializing in:

- Maintaining README, ARCHITECTURE, and markdown documentation
- Writing XML documentation comments for APIs (both public and private members)
- Ensuring documentation accuracy and clarity
- Following markdown linting and spelling standards

### Project Maintainer

**Location**: `.github/agents/project-maintainer.md`

A project maintenance specialist responsible for:

- Managing dependencies and Dependabot PRs
- Triaging and organizing issues
- Identifying improvement opportunities
- Planning enhancements and releases

### Software Quality Enforcer

**Location**: `.github/agents/software-quality-enforcer.md`

A code quality specialist focused on:

- Enforcing testing standards and code coverage
- Running static analysis and linting
- Performing code reviews and quality gates
- Ensuring zero-warning builds

## Project Structure

```text
DotnetToolWrapper/
├── .config/
│   └── dotnet-tools.json    # .NET tools configuration (sbom-tool, spdx-tool)
├── .github/
│   ├── agents/              # Specialized GitHub Copilot agent configurations
│   │   ├── documentation-writer.md
│   │   ├── project-maintainer.md
│   │   └── software-quality-enforcer.md
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
- **Code Analyzers**: Microsoft.CodeAnalysis.NetAnalyzers, SonarAnalyzer.CSharp

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
- **XML Documentation**: All code (both public and private members) should include XML documentation comments

### Workflow Structure

The project uses a modular workflow approach:

1. **build_on_push.yaml**: Triggers on every push, calls the reusable build workflow
2. **build.yaml**: Reusable workflow that performs the actual build, SBOM generation, and artifact upload
3. **release.yaml**: Handles release-specific tasks

### Quality Checks

Quality checks are automated through GitHub Actions:

- **Spelling**: Checked using `cspell` against `.cspell.json` configuration
- **Markdown Linting**: Validated using `markdownlint-cli` against `.markdownlint.json` configuration
- **Code Analysis**: Microsoft.CodeAnalysis.NetAnalyzers with enhanced rules via `.globalconfig`
- **Additional Analysis**: SonarAnalyzer.CSharp for code quality and bug detection
- **Build Validation**: Zero-warning builds enforced via `TreatWarningsAsErrors`

### Code Analysis Configuration

The project uses `.globalconfig` to configure analyzer rules globally:

- **Comprehensive Rule Set**: All CA (Code Analysis) rules configured with appropriate severity levels
- **Security Focus**: CA3xxx (security) and CA5xxx (cryptography) rules set to warning
- **Performance**: CA18xx (performance) rules enabled to catch inefficient patterns
- **Maintainability**: CA15xx (maintainability) rules track complexity and coupling
- **Best Practices**: CA1xxx (design) and CA2xxx (reliability) rules enforced

Key severity levels used:

- **warning**: Must be fixed (TreatWarningsAsErrors)
- **suggestion**: IDE hints, not build-breaking
- **none**: Rule disabled for this project

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
6. Use box-drawing characters (├──, └──, │) for file/folder diagrams rather than pipe-and-dash (|-, |-)

### When Addressing Security Issues

1. Follow the security policy outlined in `SECURITY.md`
2. Never commit sensitive information
3. Consider all supported platforms when evaluating security impact
4. Update security documentation if introducing new security considerations

## Common Tasks

### Project Configuration Philosophy

**Important**: This project prefers managing build properties, targets, and package references directly in individual
`.csproj` files rather than using centralized files like `Directory.Build.props`, `Directory.Build.targets`, or
`Directory.Packages.props`. This approach provides:

- Clear visibility of each project's configuration
- Easier debugging and understanding of project settings
- Explicit control over each project's dependencies and settings
- Reduced indirection when troubleshooting build issues

When making changes to project settings:

- Update properties directly in the relevant `.csproj` file
- Keep common settings synchronized manually between src and test projects
- Document any deviations between projects when they occur

### Adding New Dependencies

1. Update the `.csproj` file in `src/DemaConsulting.DotnetToolWrapper/`
2. If adding analyzer packages, ensure test project also gets the same version
3. Dependabot is configured to automatically update NuGet packages in the `nuget-dependencies` group
4. Do NOT use centralized package management (Directory.Packages.props) - manage versions in each .csproj

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

## Troubleshooting Guide

### Common Build Issues

#### Analyzer Warnings

- **Symptom**: Build fails with CA#### warnings
- **Solution**: Fix the code issue or adjust rule severity in `.globalconfig`
- **Note**: Never disable `TreatWarningsAsErrors` to bypass warnings

#### Multi-Framework Targeting Issues

- **Symptom**: Code works on one framework but not others
- **Solution**: Use `#if NET8_0`, `#if NET9_0`, `#if NET10_0` preprocessor directives for framework-specific code
- **Alternative**: Use runtime checks with `Environment.Version` when appropriate

#### Cross-Platform Issues

- **Symptom**: Tests pass locally but fail on CI for different OS
- **Solution**: Avoid OS-specific APIs or use runtime checks with `RuntimeInformation.IsOSPlatform()`
- **Testing**: Use CI to validate changes on all platforms before merging

### Common Test Issues

#### Integration Test Failures

- **Symptom**: Integration tests fail to find or execute native tools
- **Solution**: Ensure `CopyLocalLockFileAssemblies` is set to `true` in test project
- **Check**: Verify test project references have correct configuration

#### Platform-Specific Test Failures

- **Symptom**: Tests fail on specific OS or architecture
- **Solution**: Use conditional test execution with `[TestMethod]` attributes and runtime checks
- **Skip Tests**: Use `Assert.Inconclusive()` for platform-specific limitations

### Common CI/CD Issues

#### Workflow Failures

- **Symptom**: GitHub Actions workflow fails
- **Solution**: Check workflow logs for specific error messages
- **Tools**: Use `dotnet --info` in workflows to verify SDK versions

#### SBOM Generation Issues

- **Symptom**: SBOM tools fail during artifact generation
- **Solution**: Verify `.config/dotnet-tools.json` is properly configured
- **Check**: Ensure `dotnet tool restore` runs successfully before SBOM generation

### Dependency Management Issues

#### Version Conflicts

- **Symptom**: Analyzer package versions differ between projects
- **Solution**: Manually sync versions in both src and test `.csproj` files
- **Check**: Search for package name in both files and compare versions

#### Dependabot PR Conflicts

- **Symptom**: Dependabot creates separate PRs for src and test projects
- **Solution**: This is expected - review and merge both PRs to keep versions synchronized
- **Prevention**: Dependabot groups related packages but can't enforce cross-project consistency

## Adding New Specialized Agents

When adding a new specialized agent to the project:

1. **Create Agent File**: Add new `.md` file in `.github/agents/` directory
2. **Follow Structure**: Use YAML frontmatter with `name` and `description` fields
3. **Document Responsibilities**: Clearly define the agent's role and responsibilities
4. **Add Guidelines**: Include specific guidelines, best practices, and examples
5. **Update AGENTS.md**: Add new agent to the "Specialized Agents" section
6. **Reference Resources**: Link to relevant files, documentation, and configurations
7. **Provide Examples**: Include code examples and common scenarios

Example frontmatter:

```yaml
---
name: Agent Name
description: Brief description of agent's expertise and role
---
```

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
