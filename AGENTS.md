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
├── .config/                 # .NET tools configuration
├── .github/                 # GitHub configuration
│   ├── agents/              # Specialized GitHub Copilot agent configurations
│   ├── ISSUE_TEMPLATE/      # GitHub issue templates
│   └── workflows/           # GitHub Actions workflows
├── docs/                    # Additional documentation
├── src/                     # Source code
│   └── DemaConsulting.DotnetToolWrapper/
├── test/                    # Test project
│   └── DemaConsulting.DotnetToolWrapper.Tests/
├── .cspell.json            # Spelling check configuration
├── .editorconfig           # Editor configuration
├── .globalconfig           # Analyzer configuration
├── .markdownlint.json      # Markdown linting configuration
├── AGENTS.md               # Agent instructions
├── ARCHITECTURE.md         # Architecture documentation
├── CONTRIBUTING.md         # Contribution guidelines
├── README.md               # Project documentation
└── SECURITY.md             # Security policy
```

## Key Technologies

- **.NET 8.0, 9.0, 10.0**: Multi-targeted framework versions
- **C# 12**: Programming language
- **MSTest**: Testing framework
- **GitHub Actions**: CI/CD automation
- **Code Analyzers**: Static analysis with Microsoft and SonarAnalyzer packages

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

### Code Standards

- **Language Version**: C# 12
- **Nullable Reference Types**: Enabled
- **Target Frameworks**: net8.0, net9.0, net10.0
- **EditorConfig**: Follow .editorconfig rules for consistent formatting
- **XML Documentation**: All code should include XML documentation comments
- **Analyzers**: Keep analyzer packages at the same version across all projects

### Workflow Structure

The project uses GitHub Actions with a modular workflow approach. Workflows are located in `.github/workflows/`
and include build, test, and release automation.

### Quality Checks

Quality checks are automated through GitHub Actions:

- **Spelling**: Checked using `cspell` with `.cspell.json` configuration
- **Markdown Linting**: Validated using `markdownlint-cli` with `.markdownlint.json` configuration
- **Code Analysis**: Enforced via analyzer packages and `.globalconfig` rules
- **Build Validation**: Zero-warning builds required (`TreatWarningsAsErrors`)

## Agent Instructions

### When Working with Code

1. **Multi-targeting**: Ensure changes are compatible with .NET 8.0, 9.0, and 10.0
2. **Cross-platform**: The tool must work on Windows, Linux, FreeBSD, and macOS
3. **Architecture Support**: Support multiple architectures (x86, x64, ARM, ARM64, WASM, S390x)
4. **Testing**: Always run tests after changes. CI tests on ubuntu-latest, windows-latest, and macos-latest
5. **Code Quality**: Maintain `TreatWarningsAsErrors` - all warnings must be fixed
6. **Analyzers**: Keep analyzer packages at consistent versions across projects

### When Modifying Workflows

1. Preserve the reusable workflow pattern
2. Ensure build, test, and release automation continues to work
3. Maintain SBOM generation if present

### When Updating Documentation

1. Follow markdown linting rules in `.markdownlint.json`
2. Check spelling with `.cspell.json` dictionary
3. Keep README.md synchronized with functionality
4. Update ARCHITECTURE.md for architectural changes
5. Reference appropriate documentation files
6. Use box-drawing characters (├──, └──, │) for file/folder diagrams

### When Addressing Security Issues

1. Follow the security policy outlined in `SECURITY.md`
2. Never commit sensitive information
3. Consider all supported platforms when evaluating security impact
4. Update security documentation if introducing new security considerations

## Common Tasks

### Adding New Dependencies

1. Update the `.csproj` file in the appropriate project folder
2. Ensure analyzer packages use consistent versions across projects
3. Dependabot automatically updates NuGet packages in the `nuget-dependencies` group

### Updating Supported Frameworks

1. Modify `<TargetFrameworks>` in `.csproj` files
2. Update workflows if needed to accommodate new framework versions
3. Test on all target platforms

## Testing Changes

Before committing:

1. Build locally: `dotnet build --configuration Release`
2. Run tests: `dotnet test --configuration Release`
3. Run spelling checks: `npx cspell "**/*.md"`
4. Run markdown linting: `npx markdownlint "**/*.md"`

## Issue Templates

The repository includes structured issue templates in `.github/ISSUE_TEMPLATE/` for bug reports, feature requests,
and other items.

## Related Documentation

- <a>README.md</a> - Project overview and quick start guide
- <a>ARCHITECTURE.md</a> - Detailed architecture and design documentation
- <a>CONTRIBUTING.md</a> - Contribution guidelines and development setup
- <a>CODE_OF_CONDUCT.md</a> - Community code of conduct
- <a>SECURITY.md</a> - Security policy and vulnerability reporting
- <a>docs/usage.md</a> - Comprehensive usage guide with examples
- <a>LICENSE</a> - MIT License terms

## Contact

For questions or issues, please refer to the repository's issue tracker on GitHub.

[dotnet-tools]: https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools
