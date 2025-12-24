# DotnetToolWrapper

![GitHub forks](https://img.shields.io/github/forks/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub Repo stars](https://img.shields.io/github/stars/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub](https://img.shields.io/github/license/demaconsulting/DotnetToolWrapper?style=plastic)

A .NET console application that enables native executables to be distributed as
[.NET Tools][link1].

## Overview

DotnetToolWrapper bridges the gap between the .NET tool ecosystem and native applications. It provides a managed
.NET entry point that detects the current platform and launches the appropriate native executable, enabling developers
to distribute cross-platform command-line tools through NuGet.

## Features

- 🌍 **Multi-Platform Support** - Works on Windows, Linux, FreeBSD, and macOS
- 🏗️ **Multi-Architecture** - Supports x86, x64, ARM, ARM64, WASM, and S390x
- 🎯 **Multi-Framework** - Targets .NET 8.0, 9.0, and 10.0
- 📦 **Zero Dependencies** - No external dependencies beyond .NET runtime
- ⚙️ **Simple Configuration** - JSON-based configuration for platform-to-executable mapping
- 🔄 **Transparent Execution** - Passes arguments and exit codes through seamlessly
- 🌐 **Environment Variables** - Supports environment variable expansion in paths
- ⚡ **Minimal Overhead** - Lightweight wrapper with negligible performance impact

## Quick Start

Install a tool that uses DotnetToolWrapper:

```bash
dotnet tool install -g YourTool.Package
your-tool --help
```

Create your own wrapped tool by following the [Usage][link2] section below.

## Usage

To create a .NET tool using DotnetToolWrapper, you'll need to:

1. Create a .nuspec file for the .NET tool package
2. Create configuration files (DotnetToolSettings.xml and DotnetToolWrapper.json)
3. Copy DotnetToolWrapper files and your native executables
4. Package as a NuGet package

For complete step-by-step instructions, detailed examples, and troubleshooting, see the [Usage Guide](docs/usage.md).

## Quick Example

Here's a minimal folder structure:

```text
root
|- tool.nuspec
|- win-x64/my-tool.exe
|- linux-x64/my-tool
|- osx-arm64/my-tool
|- tools
   |- net8.0/any
   |- net9.0/any
   |- net10.0/any
      |- DotnetToolSettings.xml
      |- DotnetToolWrapper.json
      |- DemaConsulting.DotnetToolWrapper.dll
      |- (other wrapper files)
```

Package and install:

```bash
nuget pack tool.nuspec -Version 1.0.0
dotnet tool install -g My.Tool.Package
my-tool --help
```

## How It Works

1. User installs your .NET tool package via `dotnet tool install`
2. When the tool is invoked, .NET launches `DemaConsulting.DotnetToolWrapper.dll`
3. The wrapper detects the current OS and architecture
4. The wrapper reads `DotnetToolWrapper.json` to find the appropriate native executable
5. The wrapper launches the native executable with the original arguments
6. The wrapper returns the native executable's exit code

## Platform Support

### Operating Systems

- Windows
- Linux
- FreeBSD
- macOS

### Architectures

- x86 (32-bit Intel/AMD)
- x64 (64-bit Intel/AMD)
- ARM (32-bit ARM)
- ARM64 (64-bit ARM)
- WASM (WebAssembly)
- S390x (IBM System z)

### .NET Frameworks

- .NET 8.0
- .NET 9.0
- .NET 10.0

## Documentation

- **[Architecture](ARCHITECTURE.md)** - Detailed architecture and design documentation
- **[Contributing](CONTRIBUTING.md)** - Guidelines for contributing to the project
- **[Code of Conduct](CODE_OF_CONDUCT.md)** - Community standards and expectations
- **[Security](SECURITY.md)** - Security policy and vulnerability reporting

## Building from Source

```bash
# Clone the repository
git clone https://github.com/demaconsulting/DotnetToolWrapper.git
cd DotnetToolWrapper

# Restore dependencies
dotnet restore

# Build the project
dotnet build --configuration Release

# Output will be in src/DemaConsulting.DotnetToolWrapper/bin/Release/
```

## Examples

For real-world examples of tools using DotnetToolWrapper, see:

- Check the [GitHub topic][link3] for projects using this wrapper

## Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details on:

- Reporting bugs
- Suggesting enhancements
- Submitting pull requests
- Development setup
- Code style guidelines

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

- **Issues**: Report bugs or request features via [GitHub Issues][link4]
- **Discussions**: Ask questions via [GitHub Discussions][link5]
- **Security**: Report vulnerabilities per [SECURITY.md](SECURITY.md)

[link1]: https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools
[link2]: #usage
[link3]: https://github.com/topics/dotnettoolwrapper
[link4]: https://github.com/demaconsulting/DotnetToolWrapper/issues
[link5]: https://github.com/demaconsulting/DotnetToolWrapper/discussions
