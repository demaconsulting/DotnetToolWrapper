# DotnetToolWrapper

![Structure of the DotnetToolWrapper system: the Program unit](DotnetToolWrapperView.svg)

## Architecture

The .NET tool ecosystem distributes and installs command-line tools with `dotnet tool install`, but a .NET Tool
must be a .NET application. DotnetToolWrapper bridges this gap for programs written in other languages (C, C++,
Rust, Go, and others) and compiled to native executables: it is the managed entry point of a .NET Tool package,
and it launches the native executable built for the host platform.

The .NET SDK starts the wrapper as the tool's entry point, as declared in the package's `DotnetToolSettings.xml`.
The wrapper then does all of its work in a single software unit, **Program**, a static class whose `Main` method
determines the target name of the host, reads the configuration, starts the native program, and propagates its
exit code. The system has no subsystems and no system-level code; Program is its only unit, so there is no
collaboration between units to describe. The *Program* unit design describes the methods and the algorithm.

## External Interfaces

**Command line**: the arguments given to the tool command.

- *Type*: CLI.
- *Role*: Provider (the user, or a script, runs the tool command).
- *Contract*: every argument is passed to the native program unchanged and in order through
  `ProcessStartInfo.ArgumentList`. The wrapper defines no options of its own.
- *Constraints*: no shell parsing, quoting, or globbing is applied by the wrapper; any expansion has already
  been done by the caller's shell.

**`DotnetToolSettings.xml`**: the .NET Tool settings file in each framework folder of the package.

- *Type*: XML file, consumed by the .NET SDK, not by the wrapper.
- *Role*: Provider (the SDK uses it to start the wrapper).
- *Contract*: names the tool command and sets `EntryPoint="DemaConsulting.DotnetToolWrapper.dll"` and
  `Runner="dotnet"`, so the SDK starts the wrapper on the installed .NET runtime.
- *Constraints*: written by the package author; the wrapper does not read or validate it.

**`DotnetToolWrapper.json`**: the configuration file in the folder that holds the wrapper DLL.

- *Type*: JSON file.
- *Role*: Consumer (the wrapper reads it).
- *Contract*: the root is a JSON object whose property names are target names of the form `{os}-{arch}`. Each
  value is an object with a `program` string property that holds the path of the native executable for that
  target, for example `{ "linux-x64": { "program": "../../../linux-x64/my-tool" } }`. Any operating system
  name (`win`, `linux`, `freebsd`, `osx`, `unknown`) combined with any architecture name (`x86`, `x64`, `arm`,
  `arm64`, `wasm`, `s390x`, `unknown`) forms a target; the names come from the Program unit. `%NAME%`
  environment variable references in the path are expanded, and a relative path is resolved against the folder
  of the configuration file. Other targets and other properties are ignored.
- *Constraints*: the file is trusted, because it selects the program to run. It must be valid JSON; property
  names are case-sensitive.

**Native program process**: the native executable started by the wrapper.

- *Type*: operating system process.
- *Role*: Consumer (the wrapper starts it).
- *Contract*: started directly with `UseShellExecute = false`, in the caller's current working directory, with
  the caller's arguments. Standard input, output, and error are not redirected, and the environment is
  inherited. The wrapper waits for the process to exit.
- *Constraints*: the executable must exist and be executable by the user; the wrapper does not check this
  before starting it.

**Standard output**: the wrapper's own messages.

- *Type*: console text.
- *Role*: Provider.
- *Contract*: the wrapper writes one of four messages when it cannot run the program:
  `Missing configuration file {path}`, `This tool does not support the {target} target`,
  `Bad configuration for {target} target`, and `Unable to start process {path}`.
- *Constraints*: the messages go to standard output, not standard error. All other console output comes from
  the native program.

**Exit code**: the result of the tool command.

- *Type*: process exit code.
- *Role*: Provider.
- *Contract*: the exit code of the native program when it ran; 1 for each of the four wrapper messages; a
  non-zero code chosen by the .NET runtime when an unhandled exception ends the wrapper.
- *Constraints*: scripts and build systems rely on it to detect success or failure.

**Release package `DotnetToolWrapper.zip`**: the distribution of the wrapper to package authors.

- *Type*: ZIP archive attached to each GitHub release.
- *Role*: Provider.
- *Contract*: holds one folder per target framework (`net8.0`, `net9.0`, `net10.0`, `net11.0`), each with the
  same three files: `DemaConsulting.DotnetToolWrapper.dll`, `DemaConsulting.DotnetToolWrapper.deps.json`, and
  `DemaConsulting.DotnetToolWrapper.runtimeconfig.json`.
- *Constraints*: the wrapper is framework-dependent; it runs on the .NET runtime that the tool's users have
  installed.

## Dependencies

DotnetToolWrapper has no OTS or Shared Package dependencies at run time. It uses only the .NET base class library
(`System.Text.Json`, `System.Diagnostics.Process`, and `System.Runtime.InteropServices.RuntimeInformation`), which
is part of the .NET runtime it runs on.

The OTS items below build, check, test, and document the wrapper; none of them ships with it:

- **xUnit**: runs the unit and system tests - see *xUnit* in *OTS Software Integration Design*.
- **ReqStream**: checks the requirements and traces them to the tests - see *ReqStream*.
- **ReviewMark**: checks the review configuration and generates the review plan and report - see *ReviewMark*.
- **BuildMark** and **VersionMark**: generate the build notes and record the tool versions - see *BuildMark* and
  *VersionMark*.
- **TextLint** and **LintMark**: check the spelling and the Markdown formatting - see *TextLint* and *LintMark*.
- **SysML2Tools**: lints the architecture model and renders its diagrams - see *SysML2Tools*.
- **SpdxTool**: enhances and validates the software bill of materials - see *SpdxTool*.
- **Pandoc**, **CanvasNet**, **WeasyPrint**, and **FileAssert**: build and check the release documents - see
  *Pandoc*, *CanvasNet*, *WeasyPrint*, and *FileAssert*.

## Risk Control Measures

N/A - not a safety-classified software item.

## Data Flow

Each time the tool command runs, data flows from the caller through the wrapper to the native program and back:

1. The .NET SDK starts the wrapper, as configured in `DotnetToolSettings.xml`, and passes it the arguments of
   the tool command.
2. The wrapper builds the target name `{os}-{arch}` from the host operating system and processor architecture.
3. It reads `DotnetToolWrapper.json` from the folder of the wrapper DLL. When the file is missing, it writes
   `Missing configuration file {path}` and exits with code 1.
4. It looks up the entry for the target name and the entry's `program` property. When either is missing, it
   writes `This tool does not support the {target} target` or `Bad configuration for {target} target` and exits
   with code 1.
5. It expands `%NAME%` environment variable references in the program path and resolves the result against the
   folder of the configuration file.
6. It starts the program with the caller's arguments in the caller's working directory. The program reads and
   writes the caller's console directly.
7. It waits for the program to exit and exits with the program's exit code.

The *DotnetToolWrapper User Guide* shows the same decisions as a run-time flowchart in its How DotnetToolWrapper
Works chapter.

## Design Constraints

- **Platforms**: Windows, Linux, FreeBSD, and macOS, on the x86, x64, ARM, ARM64, WASM, and S390x architectures.
  A platform or architecture the wrapper does not recognize is named `unknown`, so no configuration entry
  matches it unless the package author adds one.
- **Frameworks**: framework-dependent build for `net8.0`, `net9.0`, `net10.0`, and `net11.0`, so a tool package
  can include a folder for each .NET SDK its users may have.
- **No dependencies**: nothing beyond the .NET runtime, which keeps the wrapper small and avoids version
  conflicts in tool packages.
- **No shell**: the program is started with `UseShellExecute = false`, so arguments are not re-parsed by a shell
  and behavior is the same on every operating system.
- **Working directory**: the program runs in the caller's current directory, not in the installation folder,
  so relative paths in its arguments work as the user expects.
- **Exit code**: the wrapper exits with the program's exit code, so automation sees the program's result.
- **Security**: the configuration file must be trusted, because it selects the executable to run. The wrapper
  does not validate or sanitize the arguments; it passes them through `ArgumentList`, which prevents
  re-quoting and shell interpretation. Package authors must ship native executables from trusted sources.
- **Resources**: the wrapper is single-threaded. It stays resident while the program runs, waiting for it to
  exit without doing other work.
