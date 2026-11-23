# ![DotnetToolWrapper: Run native applications as .NET Tools][hero]

<!-- IMPORTANT: All links in this file must be absolute URLs.
     This file is distributed in packages and relative links will not resolve. -->

[![GitHub forks](https://img.shields.io/github/forks/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/network/members)
[![GitHub stars](https://img.shields.io/github/stars/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/graphs/contributors)
[![License](https://img.shields.io/github/license/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/LICENSE)
[![Build](https://github.com/demaconsulting/DotnetToolWrapper/actions/workflows/build_on_push.yaml/badge.svg)](https://github.com/demaconsulting/DotnetToolWrapper/actions/workflows/build_on_push.yaml)

## Features

- 🌍 Runs on Windows, Linux, FreeBSD, and macOS
- 🏗️ Selects the native executable for x86, x64, Arm, Arm64, WASM, or S390x
- 🎯 Runs on .NET 8, 9, 10, and 11
- 📦 Has no dependencies beyond the .NET runtime
- ⚙️ Maps each `{os}-{arch}` target to its executable in one JSON file
- 🔄 Passes arguments through unchanged and returns the program's exit code

## Getting Started

1. Download `DotnetToolWrapper.zip` from the
   [releases page](https://github.com/demaconsulting/DotnetToolWrapper/releases) and copy the files of each
   framework folder (for example `net8.0/`) into the matching `tools/{framework}/any/` folder of the tool package.
2. Add a `DotnetToolWrapper.json` beside the wrapper files that maps each target to its native executable,
   relative to the file:

   ```json
   {
     "win-x64": { "program": "../../../win-x64/my-tool.exe" },
     "linux-x64": { "program": "../../../linux-x64/my-tool" },
     "osx-arm64": { "program": "../../../osx-arm64/my-tool" }
   }
   ```

3. Add a `DotnetToolSettings.xml` that names the command and makes the wrapper its entry point:

   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <DotNetCliTool Version="1">
     <Commands>
       <Command Name="my-tool" EntryPoint="DemaConsulting.DotnetToolWrapper.dll" Runner="dotnet" />
     </Commands>
   </DotNetCliTool>
   ```

4. Pack the package with a `.nuspec` of package type `DotnetTool`, then install and run the tool:

   ```bash
   nuget pack tool.nuspec -Version 1.0.0
   dotnet tool install -g My.Tool.Package --add-source ./
   my-tool --help
   ```

## More Information

- [User Guide](https://github.com/demaconsulting/DotnetToolWrapper/tree/main/docs/user_guide) - source; the
  PDF is published with each release
- [Releases](https://github.com/demaconsulting/DotnetToolWrapper/releases) - wrapper files, User Guide PDF,
  and compliance documents
- [Contributing](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/CONTRIBUTING.md)
- [Security](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/SECURITY.md)
- [License](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/LICENSE)

[hero]: https://raw.githubusercontent.com/demaconsulting/DotnetToolWrapper/main/docs/images/hero.png
