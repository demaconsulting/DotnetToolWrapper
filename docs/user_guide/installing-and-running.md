# Installing and Running a Wrapped Tool

This chapter shows how users install, run, update, and remove a tool package built with
DotnetToolWrapper. A wrapped tool is installed exactly like any other .NET Tool. The examples use
the package `My.Tool.Package` with the command `my-tool`.

## Install Globally

A global install puts the command on the user's path:

```bash
dotnet tool install -g My.Tool.Package
```

## Install to a Tool Path

To install the tool into a specific folder instead:

```bash
dotnet tool install --tool-path ./tools My.Tool.Package
```

The command is then run from that folder, for example `./tools/my-tool`.

## Install as a Local Tool

A local tool is recorded in a tool manifest in the repository, so everyone who clones the repository
gets the same version:

```bash
# Create the manifest once per repository
dotnet new tool-manifest

# Add the tool to the manifest and install it
dotnet tool install My.Tool.Package

# Restore the tools of an existing manifest
dotnet tool restore

# Run the local tool
dotnet tool run my-tool
```

## Run the Tool

Run the command with any arguments; the wrapper passes them to the native program unchanged:

```bash
my-tool --help
my-tool [arguments]
```

The native program runs in the current directory, uses the same console, and its exit code becomes
the exit code of the command.

## Update the Tool

```bash
dotnet tool update -g My.Tool.Package
```

## Uninstall the Tool

```bash
dotnet tool uninstall -g My.Tool.Package
```
