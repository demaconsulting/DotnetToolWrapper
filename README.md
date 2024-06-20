![GitHub forks](https://img.shields.io/github/forks/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub Repo stars](https://img.shields.io/github/stars/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/DotnetToolWrapper?style=plastic)
![GitHub](https://img.shields.io/github/license/demaconsulting/DotnetToolWrapper?style=plastic)

# About

This project generates a .NET 6.0 and 8.0 console application suitable for use in [Dotnet Tool](https://learn.microsoft.com/en-us/dotnet/core/tools/global-tools) packages which wrap native applications.

# Usage

To create a DotNet tool for an existing application:

1. Create a .nuspec file for the Dotnet tool
2. Create a `tools/net8.0/any/DotnetToolSettings.xml` file which points to this DotnetToolWrapper
3. Create a `tools/net8.0/any/DotnetToolWrapper.json` file describing the existing application to run
4. Copy this DotnetToolWrapper application (.dll) into the `tools/net8.0/any` folder
5. Add the existing application files under the `tools/net8.0/any` folder.
6. Package the NuGet package

## Folder Structure

The following is an example folder structure for a tool:

```
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

The `Name` should be customized to match the desired name of the Dotnet tool. Dotnet uses this information when installing the package.

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

The target strings consist of the operating system and architecture. Supported operating systems are `win`, `linux`, `freebsd`, and `osx`. Supported architectures are `x86`, `x64`, `arm`, `arm64`, `wasm`, and `s390x`.

## Packaging

To create the Dotnet tool Nuget package run `nuget pack -Version <x.y.z>` specifying the version of the package to create.
