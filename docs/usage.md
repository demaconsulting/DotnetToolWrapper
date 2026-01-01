# Usage Guide

This guide provides detailed instructions for creating a .NET tool using DotnetToolWrapper.

## Overview

DotnetToolWrapper enables you to package native executables as .NET Tools, making them distributable through NuGet
and installable via the `dotnet tool install` command. This guide walks you through the complete process.

## Prerequisites

- .NET SDK 8.0, 9.0, or 10.0
- NuGet CLI or dotnet pack command
- Your native executable(s) for target platforms

## Creating a DotNet Tool

To create a .NET tool for an existing application, follow these steps:

1. **Create a .nuspec file** - Defines the NuGet package metadata
2. **Create DotnetToolSettings.xml** - Specifies the tool command name and entry point
3. **Create DotnetToolWrapper.json** - Maps platforms to native executables
4. **Copy DotnetToolWrapper files** - Add the wrapper application files
5. **Add your native executables** - Include platform-specific binaries
6. **Package the NuGet package** - Create the final distributable package

## Folder Structure

The following is the recommended folder structure for a tool. Note that you should include folders for all
target .NET frameworks (net8.0, net9.0, and net10.0) to ensure compatibility:

```text
root/
├── tool.nuspec
├── README.md
├── win-x64/
│   └── my-tool.exe
├── linux-x64/
│   └── my-tool
├── osx-x64/
│   └── my-tool
└── tools/
    ├── net8.0/
    │   └── any/
    │       ├── DotnetToolSettings.xml
    │       ├── DotnetToolWrapper.json
    │       ├── DemaConsulting.DotnetToolWrapper.deps.json
    │       ├── DemaConsulting.DotnetToolWrapper.dll
    │       └── DemaConsulting.DotnetToolWrapper.runtimeconfig.json
    ├── net9.0/
    │   └── any/
    │       ├── DotnetToolSettings.xml
    │       ├── DotnetToolWrapper.json
    │       ├── DemaConsulting.DotnetToolWrapper.deps.json
    │       ├── DemaConsulting.DotnetToolWrapper.dll
    │       └── DemaConsulting.DotnetToolWrapper.runtimeconfig.json
    └── net10.0/
        └── any/
            ├── DotnetToolSettings.xml
            ├── DotnetToolWrapper.json
            ├── DemaConsulting.DotnetToolWrapper.deps.json
            ├── DemaConsulting.DotnetToolWrapper.dll
            └── DemaConsulting.DotnetToolWrapper.runtimeconfig.json
```

**Note**: While the example above shows net8.0, net9.0, and net10.0 folders, you can choose to include only
the framework versions you want to support. However, including all three ensures maximum compatibility across
different .NET SDK versions that users might have installed.

## Step-by-Step Guide

### 1. Create the Nuspec File

Create a `.nuspec` file that defines your package metadata:

```xml
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
    <metadata>
        <id>My.Tool.Package</id>
        <version>1.0.0</version>
        <title>My Tool</title>
        <authors>Your Name</authors>
        <license type="expression">MIT</license>
        <readme>docs/README.md</readme>
        <description>Description of your tool</description>
        <projectUrl>https://github.com/yourusername/your-tool</projectUrl>
        <repository type="git" url="https://github.com/yourusername/your-tool.git" />
        <tags>dotnet-tool cli native</tags>
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

**Key points:**

- Set a unique `<id>` for your package
- Use semantic versioning for `<version>`
- Include `<packageType name="DotnetTool" />` to mark this as a .NET tool
- Adjust file paths in the `<files>` section to match your structure

Refer to the [.nuspec File Reference][nuspec-reference] for more details.

### 2. Create DotnetToolSettings.xml

Create a `DotnetToolSettings.xml` file in each framework folder (e.g., `tools/net8.0/any/`):

```xml
<?xml version="1.0" encoding="utf-8"?>
<DotNetCliTool Version="1">
  <Commands>
    <Command Name="my-tool" EntryPoint="DemaConsulting.DotnetToolWrapper.dll" Runner="dotnet" />
  </Commands>
</DotNetCliTool>
```

**Key points:**

- Replace `my-tool` with the command name users will type to run your tool
- Keep `EntryPoint` as `DemaConsulting.DotnetToolWrapper.dll`
- Keep `Runner` as `dotnet`

### 3. Create DotnetToolWrapper.json

Create a `DotnetToolWrapper.json` file in each framework folder that maps platform identifiers to executables:

```json
{
  "win-x64": {
    "program": "../../../win-x64/my-tool.exe"
  },
  "win-x86": {
    "program": "../../../win-x86/my-tool.exe"
  },
  "linux-x64": {
    "program": "../../../linux-x64/my-tool"
  },
  "linux-arm64": {
    "program": "../../../linux-arm64/my-tool"
  },
  "osx-x64": {
    "program": "../../../osx-x64/my-tool"
  },
  "osx-arm64": {
    "program": "../../../osx-arm64/my-tool"
  }
}
```

**Platform Identifiers:**

The platform identifier format is `{os}-{architecture}`:

**Supported Operating Systems:**

- `win` - Windows
- `linux` - Linux
- `freebsd` - FreeBSD
- `osx` - macOS

**Supported Architectures:**

- `x86` - 32-bit Intel/AMD
- `x64` - 64-bit Intel/AMD
- `arm` - 32-bit ARM
- `arm64` - 64-bit ARM (Apple Silicon, etc.)
- `wasm` - WebAssembly (architecture detected, but typically requires special runtime handling)
- `s390x` - IBM System z

**Key points:**

- Only include platforms you support
- Paths are relative to the DotnetToolWrapper.json file location
- Environment variables in paths will be expanded (e.g., `%USERPROFILE%`, `$HOME`)
- The wrapper will automatically select the correct executable based on the runtime platform

### 4. Copy DotnetToolWrapper Files

For each framework version you're supporting, copy the following files from the DotnetToolWrapper release
into your `tools/{framework}/any/` folder:

- `DemaConsulting.DotnetToolWrapper.dll`
- `DemaConsulting.DotnetToolWrapper.deps.json`
- `DemaConsulting.DotnetToolWrapper.runtimeconfig.json`

You can obtain these files by:

1. Downloading from the [releases page][releases]
2. Building from source (see [Building from Source][building-from-source])

### 5. Add Your Native Executables

Place your native executables at the root of the package in subdirectories matching the platform identifiers. For example:

```text
root/
├── win-x64/
│   └── my-tool.exe
├── linux-x64/
│   └── my-tool
└── osx-arm64/
    └── my-tool
```

**Important:**

- Ensure Linux and macOS executables have execute permissions
- Test executables on their target platforms before packaging
- Consider including debug symbols or stripped binaries based on your needs

### 6. Package the NuGet Package

Once all files are in place, create the NuGet package:

```bash
nuget pack tool.nuspec -Version 1.0.0
```

Or using dotnet pack if you have a .csproj:

```bash
dotnet pack -c Release -p:PackageVersion=1.0.0
```

This creates a `.nupkg` file that can be:

- Published to NuGet.org
- Published to a private NuGet feed
- Installed locally for testing

## Installing and Using Your Tool

### Install Globally

```bash
dotnet tool install -g My.Tool.Package
```

### Install Locally

```bash
dotnet tool install --tool-path ./tools My.Tool.Package
```

### Run the Tool

```bash
my-tool --help
my-tool [arguments]
```

### Uninstall

```bash
dotnet tool uninstall -g My.Tool.Package
```

## Advanced Configuration

### Environment Variables in Paths

You can use environment variables in the `program` path within `DotnetToolWrapper.json`:

```json
{
  "win-x64": {
    "program": "%LOCALAPPDATA%/MyTool/my-tool.exe"
  },
  "linux-x64": {
    "program": "$HOME/.local/share/mytool/my-tool"
  }
}
```

This is useful when your tool needs to be installed in a user-specific location.

### Multiple Framework Targeting

If you want to support multiple .NET framework versions:

1. Create separate folders for each framework: `net8.0`, `net9.0`, `net10.0`
2. Copy the appropriate DotnetToolWrapper binaries for each framework
3. Place your native executables at the root of the package (they are shared across all frameworks)
4. Update your .nuspec to include all framework folders

Users with different .NET SDK versions will automatically use the appropriate framework version.

### Testing Locally

Before publishing, test your package locally:

```bash
# Pack the package
nuget pack tool.nuspec -Version 1.0.0-test

# Install from local file
dotnet tool install -g My.Tool.Package --add-source ./

# Test the tool
my-tool --version

# Uninstall when done
dotnet tool uninstall -g My.Tool.Package
```

## Building from Source

To build DotnetToolWrapper from source:

```bash
# Clone the repository
git clone https://github.com/demaconsulting/DotnetToolWrapper.git
cd DotnetToolWrapper

# Restore dependencies
dotnet restore

# Build for all target frameworks
dotnet build --configuration Release

# Find the output files in:
# src/DemaConsulting.DotnetToolWrapper/bin/Release/net8.0/
# src/DemaConsulting.DotnetToolWrapper/bin/Release/net9.0/
# src/DemaConsulting.DotnetToolWrapper/bin/Release/net10.0/
```

## Troubleshooting

### Tool Not Found After Installation

**Problem**: After running `dotnet tool install`, the command is not found.

**Solutions:**

- Ensure the .NET tools directory is in your PATH
  - Windows: `%USERPROFILE%\.dotnet\tools`
  - Linux/macOS: `~/.dotnet/tools`
- Restart your terminal or run `dotnet tool list -g` to verify installation

### Unsupported Platform Error

**Problem**: "This tool does not support the {platform} target"

**Solutions:**

- Verify the platform identifier in `DotnetToolWrapper.json` matches your system
- Add support for your platform by including the appropriate native executable
- Check that the platform identifier format is correct (`{os}-{architecture}`)

### Missing Configuration File

**Problem**: "Missing configuration file DotnetToolWrapper.json"

**Solutions:**

- Ensure `DotnetToolWrapper.json` is in the same directory as `DemaConsulting.DotnetToolWrapper.dll`
- Check file permissions and ensure the file is readable
- Verify the file is included in the NuGet package

### Permission Denied (Linux/macOS)

**Problem**: Permission denied when running the tool on Linux or macOS

**Solutions:**

- Ensure the native executable has execute permissions before packaging:

  ```bash
  chmod +x my-tool
  ```

- If the executable is already packaged, extract it, fix permissions, and repackage

### Wrong Executable Runs

**Problem**: The tool runs the wrong executable or fails to find the right one.

**Solutions:**

- Double-check the platform identifier in `DotnetToolWrapper.json`
- Verify the paths are correct and relative to the json file location
- Test the configuration by examining what platform the wrapper detects:
  - The wrapper constructs the platform as `{GetOs()}-{GetArchitecture()}`

## Examples

For real-world examples of tools using DotnetToolWrapper:

- Browse repositories on GitHub with the [dotnettoolwrapper topic][github-topics]
- Check the examples in the DotnetToolWrapper repository (if available)

## Support

- **Issues**: Report bugs or request features via [GitHub Issues][github-issues]
- **Discussions**: Ask questions via [GitHub Discussions][github-discussions]
- **Documentation**: See [README.md](../README.md) for project overview
- **Architecture**: See [ARCHITECTURE.md](../ARCHITECTURE.md) for design details
- **Contributing**: See [CONTRIBUTING.md](../CONTRIBUTING.md) for contribution guidelines

## Additional Resources

- [.NET Tool Documentation][dotnet-tools]
- [.nuspec File Reference][nuspec-reference]
- [NuGet Package Creation][nuget-package-creation]
- [NuGet CLI Reference][nuget-cli-reference]

[nuspec-reference]: https://learn.microsoft.com/en-us/nuget/reference/nuspec
[releases]: https://github.com/demaconsulting/DotnetToolWrapper/releases
[building-from-source]: #building-from-source
[github-topics]: https://github.com/topics/dotnettoolwrapper
[github-issues]: https://github.com/demaconsulting/DotnetToolWrapper/issues
[github-discussions]: https://github.com/demaconsulting/DotnetToolWrapper/discussions
[dotnet-tools]: https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools
[nuget-package-creation]: https://learn.microsoft.com/en-us/nuget/create-packages/creating-a-package
[nuget-cli-reference]: https://learn.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference
