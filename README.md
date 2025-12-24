# DotnetToolWrapper

![GitHub forks](https://img.shields.io/github/forks/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub Repo stars](https://img.shields.io/github/stars/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub](https://img.shields.io/github/license/demaconsulting/DotnetToolWrapper?style=plastic)

A .NET console application that enables native executables to be distributed as
[.NET Tools](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools).

## Overview

DotnetToolWrapper bridges the gap between the .NET tool ecosystem and native applications. It provides a managed
.NET entry point that detects the current platform and launches the appropriate native executable, enabling developers
to distribute cross-platform command-line tools through NuGet.

## Features

| Feature                    | Description                                              |
| -------------------------- | -------------------------------------------------------- |
| **Multi-Platform Support** | Works on Windows, Linux, FreeBSD, and macOS              |
| **Multi-Architecture**     | Supports x86, x64, ARM, ARM64, WASM, and S390x           |
| **Multi-Framework**        | Targets .NET 8.0, 9.0, and 10.0                          |
| **Zero Dependencies**      | No external dependencies beyond .NET runtime             |
| **Simple Configuration**   | JSON-based configuration for platform-to-executable map  |
| **Transparent Execution**  | Passes arguments and exit codes through seamlessly       |
| **Environment Variables**  | Supports environment variable expansion in paths         |
| **Minimal Overhead**       | Lightweight wrapper with negligible performance impact   |

## Quick Start

Install a tool that uses DotnetToolWrapper:

```bash
dotnet tool install -g YourTool.Package
your-tool --help
```

Create your own wrapped tool by following the [Usage](#usage) section below.

## Usage

To create a DotNet tool for an existing application:

1. Create a .nuspec file for the Dotnet tool
2. Create a `tools/net8.0/any/DotnetToolSettings.xml` file which points to this DotnetToolWrapper
3. Create a `tools/net8.0/any/DotnetToolWrapper.json` file describing the existing application to run
4. Copy this DotnetToolWrapper application (.dll) into the `tools/net8.0/any` folder
5. Add the existing application files under the `tools/net8.0/any` folder.
6. Package the NuGet package

## Folder Structure

The following is an example folder structure for a tool:

```text
root
|- tool.nuspec                                                   Nuspec file
|- README.md                                                     README file
|- tools
   |- net8.0
      |- any
         |- DotnetToolSettings.xml                               Dotnet tool settings
         |- DotnetToolWrapper.json                               DotnetToolWrapper application settings
         |- DemaConsulting.DotnetToolWrapper.deps.json           DotnetToolWrapper dependencies
         |- DemaConsulting.DotnetToolWrapper.dll                 DotnetToolWrapper application
         |- DemaConsulting.DotnetToolWrapper.runtimeconfig.json  DotnetToolWrapper runtime
```

## Nuspec File

The following is a sample .nuspec file for a tool:

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
    <metadata>
        <id>My.Tool.Package</id>
        <version>0.0.0</version>
        <title>Title of Tool</title>
        <authors>Author Name</authors>
        <license type="expression">MIT</license>
        <readme>docs/README.md</readme>
        <description>Description of Tool</description>
        <packageTypes>
            <packageType name="DotnetTool" />
        </packageTypes>
    </metadata>
    <files>
        <file src="README.md" target="docs/README.md" />
        <file src="tools/**/*" target="tools" />
    </files>
</package>
```

Refer to the [.nuspec File Reference](https://learn.microsoft.com/en-us/nuget/reference/nuspec) for more details.

## DotnetToolSettings.xml

The following is a sample DotnetToolSettings.xml file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<DotNetCliTool Version="1">
  <Commands>
    <Command Name="my-tool" EntryPoint="DemaConsulting.DotnetToolWrapper.dll" Runner="dotnet" />
  </Commands>
</DotNetCliTool>
```

The `Name` should be customized to match the desired name of the Dotnet tool. Dotnet uses this information
when installing the package.

## DotnetToolWrapper.json

The following is a sample DotnetToolWrapper.json file indicating the program to execute for each supported target:

```json
{
  "win-x64": {
    "program": "win-x64/my-program.exe"
  },
  "linux-x64": {
    "program": "linux-x64/my-program"
  }
}
```

The target strings consist of the operating system and architecture. Supported operating systems are `win`,
`linux`, `freebsd`, and `osx`. Supported architectures are `x86`, `x64`, `arm`, `arm64`, `wasm`, and `s390x`.

## Packaging

To create the Dotnet tool NuGet package:

```bash
nuget pack -Version <x.y.z>
```

Replace `<x.y.z>` with your desired version number.

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

- Check the [GitHub topic](https://github.com/topics/dotnettoolwrapper) for projects using this wrapper

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

- **Issues**: Report bugs or request features via [GitHub Issues](https://github.com/demaconsulting/DotnetToolWrapper/issues)
- **Discussions**: Ask questions via [GitHub Discussions](https://github.com/demaconsulting/DotnetToolWrapper/discussions)
- **Security**: Report vulnerabilities per [SECURITY.md](SECURITY.md)

## Acknowledgments

- Built with [.NET](https://dotnet.microsoft.com/)
- Distributed via [NuGet](https://www.nuget.org/)
- CI/CD powered by [GitHub Actions](https://github.com/features/actions)
