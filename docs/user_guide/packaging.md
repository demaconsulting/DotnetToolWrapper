# Creating a Wrapped Tool Package

This chapter walks a package author through building a NuGet tool package that wraps a native program.

## Prerequisites

- The .NET SDK 8.0, 9.0, 10.0, or 11.0
- The NuGet CLI (`nuget`), or a project that can be packed with `dotnet pack`
- The native executables for each platform the tool supports
- `DotnetToolWrapper.zip` from the DotnetToolWrapper releases page

## Overview of the Steps

1. Create a `.nuspec` file that defines the NuGet package metadata.
2. Create `DotnetToolSettings.xml`, which names the tool command and its entry point.
3. Create `DotnetToolWrapper.json`, which maps each platform to its native executable.
4. Copy the DotnetToolWrapper files into the package.
5. Add the native executables.
6. Pack the NuGet package.

## Folder Structure

The following is the recommended folder structure for a tool package. It includes a folder for each
.NET version the wrapper is built for (net8.0, net9.0, net10.0, and net11.0), so the tool installs on
any of these SDK versions:

```text
root/
├── tool.nuspec
├── README.md
├── win-x64/
│   └── my-tool.exe
├── linux-x64/
│   └── my-tool
├── osx-arm64/
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
    │       └── (the same five files)
    ├── net10.0/
    │   └── any/
    │       └── (the same five files)
    └── net11.0/
        └── any/
            └── (the same five files)
```

A package may include only the .NET versions it wants to support. Including all four gives the widest
compatibility with the .NET SDK versions that users have installed.

## Step 1: Create the Nuspec File

Create a `.nuspec` file that defines the package metadata:

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
        <file src="win-x64/**/*" target="win-x64" />
        <file src="linux-x64/**/*" target="linux-x64" />
        <file src="osx-arm64/**/*" target="osx-arm64" />
    </files>
</package>
```

Key points:

- Give the package a unique `<id>`.
- Use semantic versioning for `<version>`.
- Include `<packageType name="DotnetTool" />` to mark the package as a .NET Tool.
- List every folder of native executables in the `<files>` section, and adjust the paths to match the
  folder structure.

The .nuspec file reference listed in the Introduction describes every element.

## Step 2: Create DotnetToolSettings.xml

Create a `DotnetToolSettings.xml` file in each framework folder, for example `tools/net8.0/any/`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<DotNetCliTool Version="1">
  <Commands>
    <Command Name="my-tool" EntryPoint="DemaConsulting.DotnetToolWrapper.dll" Runner="dotnet" />
  </Commands>
</DotNetCliTool>
```

Key points:

- Replace `my-tool` with the command name users type to run the tool.
- Keep `EntryPoint` as `DemaConsulting.DotnetToolWrapper.dll`.
- Keep `Runner` as `dotnet`.

## Step 3: Create DotnetToolWrapper.json

Create a `DotnetToolWrapper.json` file in each framework folder, next to the wrapper files. It maps
each supported target to the native executable to run:

```json
{
  "win-x64": {
    "program": "../../../win-x64/my-tool.exe"
  },
  "linux-x64": {
    "program": "../../../linux-x64/my-tool"
  },
  "osx-arm64": {
    "program": "../../../osx-arm64/my-tool"
  }
}
```

The relative paths go up from `tools/{framework}/any/` to the package root, where the native
executables are. The Configuration Reference chapter describes the file format, path resolution,
and environment variables in full.

## Step 4: Copy the DotnetToolWrapper Files

Download `DotnetToolWrapper.zip` from the DotnetToolWrapper releases page. It holds one folder for
each .NET version:

```text
DotnetToolWrapper.zip
├── net8.0/
├── net9.0/
├── net10.0/
└── net11.0/
```

Each folder contains the same three files, built for that .NET version:

- `DemaConsulting.DotnetToolWrapper.dll`
- `DemaConsulting.DotnetToolWrapper.deps.json`
- `DemaConsulting.DotnetToolWrapper.runtimeconfig.json`

Copy the files of each folder into the matching `tools/{framework}/any/` folder of the package, for
example the files of `net8.0/` into `tools/net8.0/any/`.

## Step 5: Add the Native Executables

Place the native executables at the root of the package, in folders named after their target, for
example:

```text
root/
├── win-x64/
│   └── my-tool.exe
├── linux-x64/
│   └── my-tool
└── osx-arm64/
    └── my-tool
```

Important:

- Give the Linux, FreeBSD, and macOS executables execute permission before packing:

  ```bash
  chmod +x linux-x64/my-tool osx-arm64/my-tool
  ```

- Test each executable on its target platform before packing.
- Decide whether to ship debug symbols or stripped binaries.

The native executables are shared by all framework folders, so they appear only once in the package.

## Step 6: Pack the NuGet Package

When all files are in place, create the NuGet package:

```bash
nuget pack tool.nuspec -Version 1.0.0
```

Or, when the package is defined by a project file:

```bash
dotnet pack -c Release -p:PackageVersion=1.0.0
```

The resulting `.nupkg` file can be published to NuGet.org or to a private NuGet feed, or installed
locally for testing.

## Testing Locally

Before publishing, test the package locally:

```bash
# Pack the package
nuget pack tool.nuspec -Version 1.0.0-test

# Install from the local folder
dotnet tool install -g My.Tool.Package --add-source ./ --version 1.0.0-test

# Test the tool
my-tool --version

# Uninstall when done
dotnet tool uninstall -g My.Tool.Package
```

## Supporting Multiple .NET Versions

To support several .NET versions:

1. Create a folder for each version: `tools/net8.0/any/`, `tools/net9.0/any/`, `tools/net10.0/any/`,
   and `tools/net11.0/any/`.
2. Copy the wrapper files built for each version into its folder, together with
   `DotnetToolSettings.xml` and `DotnetToolWrapper.json`.
3. Keep the native executables at the root of the package, shared by all versions.
4. Make sure the `.nuspec` file includes all the framework folders.

The .NET SDK picks the framework folder that matches the runtime it uses, so users with different SDK
versions all get a working tool.

## Building the Wrapper from Source

The release zip is the normal source of the wrapper files. To build them from source instead:

```bash
# Clone the repository
git clone https://github.com/demaconsulting/DotnetToolWrapper.git
cd DotnetToolWrapper

# Build and test for all target frameworks
pwsh ./build.ps1 -All
# Or build only
dotnet build DemaConsulting.DotnetToolWrapper.slnx --configuration Release
```

The output files are in:

- `src/DemaConsulting.DotnetToolWrapper/bin/Release/net8.0/`
- `src/DemaConsulting.DotnetToolWrapper/bin/Release/net9.0/`
- `src/DemaConsulting.DotnetToolWrapper/bin/Release/net10.0/`
- `src/DemaConsulting.DotnetToolWrapper/bin/Release/net11.0/`
