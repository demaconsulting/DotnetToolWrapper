# Contributing to DotnetToolWrapper

Thank you for your interest in contributing to DotnetToolWrapper! This document provides guidelines and instructions
for contributing to this project.

## Code of Conduct

This project adheres to the Contributor Covenant Code of Conduct. By participating, you are expected to uphold this
code. Please report unacceptable behavior through GitHub's issue reporting system or by contacting the repository
maintainers directly. See [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) for details.

## How Can I Contribute?

### Reporting Bugs

Before creating a bug report, please check the existing issues to avoid duplicates. When creating a bug report,
include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples** (code snippets, configuration files, etc.)
- **Describe the behavior you observed and what you expected**
- **Include environment details**:
  - Operating system and version
  - .NET SDK version (`dotnet --version`)
  - DotnetToolWrapper version
  - Architecture (x86, x64, ARM, ARM64, WASM, S390x)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide a detailed description** of the proposed feature
- **Explain why this enhancement would be useful** to most users
- **List any alternatives you've considered**
- **Include mockups or examples** if applicable

### Pull Requests

We actively welcome pull requests! To submit a pull request:

1. Fork the repository and create your branch from `main`
2. Make your changes following the code style guidelines
3. Ensure your code builds without errors
4. Test your changes on multiple platforms if possible
5. Update documentation as needed
6. Submit the pull request with a clear description of your changes

## Development Setup

### Prerequisites

- [.NET SDK 8.0 or higher][dotnet-download]
- Git
- A code editor (Visual Studio, Visual Studio Code, Rider, etc.)

### Building the Project

```bash
# Clone the repository
git clone https://github.com/demaconsulting/DotnetToolWrapper.git
cd DotnetToolWrapper

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Build for specific framework
dotnet build --configuration Release --framework net8.0
```

### Project Structure

Key files and directories:

```text
DotnetToolWrapper/
├── .config/
│   └── dotnet-tools.json    # .NET tools configuration
├── .github/
│   ├── ISSUE_TEMPLATE/      # GitHub issue templates
│   ├── dependabot.yml       # Dependabot configuration
│   └── workflows/           # CI/CD workflows
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
├── .cspell.json             # Spelling check configuration
├── .editorconfig            # Editor configuration
├── .gitignore               # Git ignore patterns
├── .markdownlint.json       # Markdown linting rules
├── AGENTS.md                # GitHub Copilot agent instructions
├── ARCHITECTURE.md          # Architecture documentation
├── CODE_OF_CONDUCT.md       # Code of conduct
├── CONTRIBUTING.md          # This file
├── DemaConsulting.DotnetToolWrapper.sln
├── LICENSE                  # MIT License
├── README.md                # Project overview
├── SECURITY.md              # Security policy
└── spdx-workflow.yaml       # SBOM enhancement workflow
```

## Code Style and Standards

### C# Coding Standards

- Follow standard C# naming conventions
- Use C# 12 language features appropriately
- Enable nullable reference types
- Use implicit usings where appropriate
- Write clear, self-documenting code
- Add XML documentation comments for all code (both public and private members)

### Code Quality

- Ensure code compiles without warnings
- Maintain compatibility with .NET 8.0, 9.0, and 10.0
- Keep the codebase simple and maintainable
- Avoid external dependencies unless absolutely necessary

### Documentation Standards

- Follow markdown linting rules (`.markdownlint.json`)
- Check spelling using the cspell dictionary (`.cspell.json`)
- Keep documentation in sync with code changes
- Use clear, concise language
- Include code examples where helpful
- Use box-drawing characters (├──, └──, │) for file/folder diagrams rather than pipe-and-dash (|-, |-)

### Testing Your Changes

```bash
# Run spelling check
npx cspell "**/*.md"

# Run markdown linting
npx markdownlint "**/*.md"

# Build all target frameworks
dotnet build --configuration Release
```

## Commit Guidelines

### Commit Messages

- Use clear and meaningful commit messages
- Start with a verb in imperative mood (e.g., "Add", "Fix", "Update")
- Keep the first line under 72 characters
- Add detailed explanation in the body if needed

Examples:

```text
Add support for PowerPC architecture

Update documentation for clarity

Fix crash when configuration file is missing
```

### Commit Scope

- Keep commits focused on a single change
- Separate refactoring from functional changes
- Update tests and documentation in the same commit as code changes

## Review Process

### What to Expect

- Maintainers will review your pull request as soon as possible
- You may receive feedback or requests for changes
- Be responsive to review comments
- Once approved, your changes will be merged

### Review Criteria

Pull requests are evaluated based on:

- **Correctness**: Does it work as intended?
- **Quality**: Is the code well-written and maintainable?
- **Compatibility**: Does it maintain backward compatibility?
- **Documentation**: Is it properly documented?
- **Testing**: Has it been tested adequately?
- **Scope**: Is the change focused and minimal?

## Platform Testing

DotnetToolWrapper must work across multiple platforms. If you don't have access to all platforms, that's okay!
Mention this in your pull request, and maintainers can help with cross-platform testing.

### Supported Platforms

- **Operating Systems**: Windows, Linux, FreeBSD, macOS
- **Architectures**: x86, x64, ARM, ARM64, WASM, S390x
- **Frameworks**: .NET 8.0, 9.0, 10.0

### Testing Checklist

When testing changes:

- [ ] Builds successfully for all target frameworks
- [ ] Works on Windows (tested locally or via CI)
- [ ] Works on Linux (tested locally or via CI)
- [ ] Works on macOS (tested locally or via CI)
- [ ] Handles missing configuration gracefully
- [ ] Preserves exit codes correctly
- [ ] Passes command-line arguments correctly

Note: CI automatically tests on Windows, Linux, and macOS platforms.

## Release Process

Releases are managed by project maintainers using GitHub Actions:

1. Version number is updated in the project file
2. Tag is created in the format `vX.Y.Z`
3. GitHub Actions builds and publishes release artifacts
4. Release notes are added to the GitHub release

## Getting Help

- **Questions**: Open a GitHub issue with the "question" label
- **Discussions**: Use GitHub Discussions for general topics
- **Documentation**: Check [README.md](README.md) and [ARCHITECTURE.md](ARCHITECTURE.md)

## Recognition

Contributors will be recognized in:

- GitHub's contributor list
- Release notes (for significant contributions)
- Project documentation (where appropriate)

## License

By contributing to DotnetToolWrapper, you agree that your contributions will be licensed under the MIT License.
See [LICENSE](LICENSE) for details.

## Additional Resources

- [.NET Tool Documentation][dotnet-tools]
- [NuGet Package Documentation][nuget-docs]
- [GitHub Flow][github-flow]
- [Writing Good Commit Messages][git-commit-guide]

Thank you for contributing to DotnetToolWrapper! 🎉

[dotnet-download]: https://dotnet.microsoft.com/download
[dotnet-tools]: https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools
[nuget-docs]: https://learn.microsoft.com/en-us/nuget/
[github-flow]: https://guides.github.com/introduction/flow/
[git-commit-guide]: https://chris.beams.io/posts/git-commit/
