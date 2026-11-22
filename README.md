# ![DotnetToolWrapper: Run native applications as .NET Tools][hero]

<!-- IMPORTANT: All links in this file must be absolute URLs.
     This file is distributed in packages and relative links will not resolve. -->

[![GitHub forks](https://img.shields.io/github/forks/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/network/members)
[![GitHub stars](https://img.shields.io/github/stars/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/stargazers)
[![GitHub contributors](https://img.shields.io/github/contributors/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/graphs/contributors)
[![License](https://img.shields.io/github/license/demaconsulting/DotnetToolWrapper?style=plastic)](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/LICENSE)
[![Build](https://github.com/demaconsulting/DotnetToolWrapper/actions/workflows/build_on_push.yaml/badge.svg)](https://github.com/demaconsulting/DotnetToolWrapper/actions/workflows/build_on_push.yaml)

## Overview

DotnetToolWrapper lets a native command-line program be distributed through NuGet and installed with
`dotnet tool install`, like any other [.NET Tool](https://learn.microsoft.com/dotnet/core/tools/global-tools).
The wrapper is the managed entry point of the tool package: when the tool runs, it detects the operating system
and processor architecture, looks up the matching native executable in `DotnetToolWrapper.json`, and runs it.

Package authors build one tool package for every platform they support, and users install and update the tool
with the .NET command-line tools they already have.

## Features

- Selects the native executable by operating system (Windows, Linux, FreeBSD, macOS) and architecture (x86,
  x64, ARM, ARM64, WASM, S390x)
- Passes the arguments to the program unchanged, runs it in the caller's working directory, and returns its
  exit code
- Uses one JSON file for configuration, with paths relative to the file and `%NAME%` environment variables
- Runs on .NET 8.0, 9.0, 10.0, and 11.0
- Has no dependencies beyond the .NET runtime
- Reports configuration errors with a clear message and exit code 1
- Traces its requirements to passing tests on every build, and ships a User Guide, a Software Design Document,
  a Software Verification Design Document, a Requirements Document, a Trace Matrix, a Review Plan, a Review Report,
  Build Notes, and a Code Quality Report with each release

## Installation

**Package authors** download `DotnetToolWrapper.zip` from the
[releases page](https://github.com/demaconsulting/DotnetToolWrapper/releases). It holds the wrapper built for
each .NET version, and each folder holds the same files:

```text
DotnetToolWrapper.zip
├── net8.0/
│   ├── DemaConsulting.DotnetToolWrapper.dll
│   ├── DemaConsulting.DotnetToolWrapper.deps.json
│   └── DemaConsulting.DotnetToolWrapper.runtimeconfig.json
├── net9.0/                (the same files)
├── net10.0/               (the same files)
└── net11.0/               (the same files)
```

**Users of a wrapped tool** install it like any other .NET Tool:

```bash
dotnet tool install -g My.Tool.Package
```

## Usage

A wrapped tool package puts the wrapper files, `DotnetToolSettings.xml`, and `DotnetToolWrapper.json` in each
framework folder, and the native executables at the package root:

```text
root/
├── tool.nuspec
├── win-x64/
│   └── my-tool.exe
├── linux-x64/
│   └── my-tool
├── osx-arm64/
│   └── my-tool
└── tools/
    ├── net8.0/any/          (the same files)
    ├── net9.0/any/          (the same files)
    ├── net10.0/any/         (the same files)
    └── net11.0/any/
        ├── DotnetToolSettings.xml
        ├── DotnetToolWrapper.json
        └── DemaConsulting.DotnetToolWrapper.* (the three wrapper files)
```

`DotnetToolWrapper.json` maps each `{os}-{arch}` target to its native executable, relative to the file:

```json
{
  "win-x64": { "program": "../../../win-x64/my-tool.exe" },
  "linux-x64": { "program": "../../../linux-x64/my-tool" },
  "osx-arm64": { "program": "../../../osx-arm64/my-tool" }
}
```

`DotnetToolSettings.xml` names the command and makes the wrapper its entry point:

```xml
<?xml version="1.0" encoding="utf-8"?>
<DotNetCliTool Version="1">
  <Commands>
    <Command Name="my-tool" EntryPoint="DemaConsulting.DotnetToolWrapper.dll" Runner="dotnet" />
  </Commands>
</DotNetCliTool>
```

Pack the package, then install and run the tool:

```bash
nuget pack tool.nuspec -Version 1.0.0
dotnet tool install -g My.Tool.Package --add-source ./
my-tool --help
```

## Building

```pwsh
pwsh ./build.ps1
```

## User Guide

The DotnetToolWrapper User Guide is available on the
[DotnetToolWrapper releases page](https://github.com/demaconsulting/DotnetToolWrapper/releases).

## Contributing

See [CONTRIBUTING.md](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/CONTRIBUTING.md) for
guidelines, and the
[Code of Conduct](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/CODE_OF_CONDUCT.md) for
community standards.

## License

This project is licensed under the MIT License; see
[LICENSE](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/LICENSE).

## Support

- [Report a bug or request a feature](https://github.com/demaconsulting/DotnetToolWrapper/issues)
- [Ask a question or start a discussion](https://github.com/demaconsulting/DotnetToolWrapper/discussions)
- [Report a security vulnerability](https://github.com/demaconsulting/DotnetToolWrapper/blob/main/SECURITY.md)

[hero]: https://raw.githubusercontent.com/demaconsulting/DotnetToolWrapper/main/docs/images/hero.png
