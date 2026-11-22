# Introduction

This guide describes how to package a native program as a .NET Tool with DotnetToolWrapper, and how to
install and run the resulting tool.

## Purpose

DotnetToolWrapper lets a native command-line program be distributed through NuGet and installed with
the `dotnet tool install` command, like any other .NET Tool. The wrapper is a small .NET console
application that serves as the entry point of the tool package. When the tool runs, the wrapper finds
the native executable built for the current operating system and processor architecture, starts it with
the same arguments, and returns its exit code.

Package authors get one distribution channel for every platform they support, and users of the tool
install and update it with the .NET command-line tools they already have.

## Scope

This guide covers:

- how the wrapper works and how it selects the native executable
- how to build a wrapped tool package from the DotnetToolWrapper release files
- the reference for the `DotnetToolWrapper.json` configuration file
- how to install, run, update, and uninstall a wrapped tool
- troubleshooting of common problems and of the wrapper's error messages

The guide is written for package authors, who build the tool packages, and for the users of those
tools. It assumes the .NET SDK 8.0, 9.0, 10.0, or 11.0 is installed, and that the package author has
the native executables for each platform the tool supports.

## References

- [DotnetToolWrapper releases](https://github.com/demaconsulting/DotnetToolWrapper/releases)
- [.NET tools overview](https://learn.microsoft.com/dotnet/core/tools/global-tools)
- [.nuspec file reference](https://learn.microsoft.com/nuget/reference/nuspec)
- [Creating a NuGet package](https://learn.microsoft.com/nuget/create-packages/creating-a-package)
- [NuGet CLI reference](https://learn.microsoft.com/nuget/reference/nuget-exe-cli-reference)
