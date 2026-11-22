## Program

![Structure of the DotnetToolWrapper system: the Program unit](DotnetToolWrapperView.svg)

### Purpose

Program is the entry point of the wrapper and its only unit. Its single responsibility is to run the native
program configured for the host platform: it maps the host operating system and processor architecture to a
target name, reads the program for that target from `DotnetToolWrapper.json`, runs it with the caller's
arguments and working directory, and exits with its exit code.

### Data Model

Program is a static class with no fields or properties. All per-invocation state is held in local variables of
`Main` and lives only for the run of the tool command. The unit consumes the following configuration model from
`DotnetToolWrapper.json`, read with `JsonDocument`:

**root**: JSON object - maps target names to target entries. Only the property named after the host target is
read; other properties are ignored.

**target property**: JSON object, named `{os}-{arch}` - the entry for one target. The name is matched exactly and
case-sensitively against the value of `GetTarget()`.

**program**: string property of the target entry - the path of the native executable. It may contain `%NAME%`
environment variable references, and it is either absolute or relative to the folder of the configuration file.
It is read with `JsonElement.ToString()`: a JSON string yields its string value, and any other JSON value
yields its raw JSON text.

### Key Methods

**`GetOs()`**: returns the operating system name of the host.

- *Parameters*: none.
- *Returns*: `string` - `win`, `linux`, `freebsd`, `osx`, or `unknown`.
- *Preconditions*: none.
- *Postconditions*: same result as `GetOs(RuntimeInformation.IsOSPlatform)`.

**`GetOs(Func<OSPlatform, bool> isOsPlatform)`**: maps a platform check to an operating system name.

- *Parameters*: `Func<OSPlatform, bool> isOsPlatform` - returns true when the host runs the given platform.
- *Returns*: `string` - `win`, `linux`, `freebsd`, `osx`, or `unknown`.
- *Preconditions*: `isOsPlatform` is not null.
- *Postconditions*: no side effects other than calling `isOsPlatform`; throws `ArgumentNullException` when
  `isOsPlatform` is null.

The platforms are checked in the order Windows, Linux, FreeBSD, then macOS (`OSPlatform.OSX`), and the first
match wins; `unknown` is returned when none matches. This internal overload is the test seam for the operating
system mapping.

**`GetArchitecture()`**: returns the architecture name of the host.

- *Parameters*: none.
- *Returns*: `string` - one of the names in the table below.
- *Preconditions*: none.
- *Postconditions*: same result as `GetArchitecture(RuntimeInformation.OSArchitecture)`.

**`GetArchitecture(Architecture architecture)`**: maps a processor architecture to an architecture name.

- *Parameters*: `Architecture architecture` - the processor architecture; any value is accepted.
- *Returns*: `string` - the architecture name.
- *Preconditions*: none.
- *Postconditions*: pure function, no side effects.

| `Architecture` value | Name |
| :--- | :--- |
| `X86` | `x86` |
| `X64` | `x64` |
| `Arm` | `arm` |
| `Arm64` | `arm64` |
| `Wasm` | `wasm` |
| `S390x` | `s390x` |
| Any other, for example `LoongArch64`, `Armv6`, `Ppc64le`, or an undefined value | `unknown` |

**`GetTarget()`**: returns the target name of the host.

- *Parameters*: none.
- *Returns*: `string` - the target name, for example `linux-x64`.
- *Preconditions*: none.
- *Postconditions*: same result as `GetTarget(RuntimeInformation.IsOSPlatform, RuntimeInformation.OSArchitecture)`.

**`GetTarget(Func<OSPlatform, bool> isOsPlatform, Architecture architecture)`**: builds a target name.

- *Parameters*: `Func<OSPlatform, bool> isOsPlatform` - the platform check; `Architecture architecture` - the
  processor architecture.
- *Returns*: `string` - `{os}-{arch}` from `GetOs(isOsPlatform)` and `GetArchitecture(architecture)`, for
  example `linux-x64` or `unknown-unknown`.
- *Preconditions*: `isOsPlatform` is not null.
- *Postconditions*: throws `ArgumentNullException` when `isOsPlatform` is null.

The three public parameterless methods call the internal overloads with `RuntimeInformation.IsOSPlatform` and
`RuntimeInformation.OSArchitecture`. The internal overloads are the test seam: `InternalsVisibleTo` exposes them
to the test project, so the tests can simulate every supported operating system and architecture on any host.

**`Main(string[] args)`**: runs the configured program and exits with its exit code.

- *Parameters*: `string[] args` - the arguments of the tool command.
- *Returns*: `void` - the method never returns normally; it ends the process with `Environment.Exit`.
- *Preconditions*: started by the .NET host as the entry point of the wrapper.
- *Postconditions*: the process exits with the program's exit code, with code 1 after a wrapper message, or with
  a runtime-defined non-zero code after an unhandled exception.

The algorithm:

1. Get the target name with `GetTarget()`.
2. Get the caller's working directory with `Directory.GetCurrentDirectory()`.
3. Get the wrapper folder from the location of the assembly (`typeof(Program).Assembly.Location`); when the
   location is empty, use the working directory instead.
4. Build the configuration path `DotnetToolWrapper.json` in the wrapper folder. When the file does not exist,
   write `Missing configuration file {path}` and exit with code 1.
5. Parse the file with `JsonDocument.Parse(File.ReadAllText(...))`.
6. Look up the target property in the root object with `TryGetProperty`. When it is missing, write
   `This tool does not support the {target} target` and exit with code 1.
7. Look up the `program` property in the target entry. When it is missing, write
   `Bad configuration for {target} target` and exit with code 1.
8. Convert the program value to a string, expand `%NAME%` references with
   `Environment.ExpandEnvironmentVariables`, and resolve the result against the wrapper folder with
   `Path.GetFullPath(programName, location)`. Only the program path is expanded, never the arguments.
9. Create a `ProcessStartInfo` for the program with `WorkingDirectory` set to the caller's working directory and
   `UseShellExecute = false`, and add every argument to `ArgumentList` unchanged and in order. Standard input,
   output, and error are not redirected and the environment is inherited.
10. Start the process with `Process.Start`. When it returns `null`, write `Unable to start process {path}` and
    exit with code 1.
11. Wait for the process to exit with `WaitForExit`, then exit with its exit code through `Environment.Exit`.

### Error Handling

Program handles four conditions itself. Each writes a message to standard output with `Console.WriteLine` and
ends the process with `Environment.Exit(1)`:

- the configuration file does not exist (`Missing configuration file {path}`);
- the configuration has no entry for the target (`This tool does not support the {target} target`);
- the target entry has no `program` property (`Bad configuration for {target} target`);
- `Process.Start` returns `null` (`Unable to start process {path}`).

The last branch is defensive. With `UseShellExecute = false`, .NET does not return `null` from `Process.Start`;
it reports a program that cannot be started by throwing an exception instead.

All other errors are not caught. They propagate out of `Main`, and the .NET runtime prints the exception and
ends the process with a non-zero exit code:

- `JsonException` when the configuration file is not valid JSON;
- `InvalidOperationException` from `TryGetProperty` when the root or the target value is not a JSON object;
- `IOException` or `UnauthorizedAccessException` when the configuration file cannot be read;
- `System.ComponentModel.Win32Exception` when the program cannot be started, for example because it is missing,
  is not executable, or the user lacks execute permission.

Propagating these errors keeps the unit simple and gives the user the full .NET description of the problem.

### Dependencies

- **RuntimeInformation** (.NET base class library) - detects the host operating system and processor
  architecture.
- **Directory**, **File**, and **Path** (.NET base class library) - get the working directory, check and read the
  configuration file, and resolve the program path.
- **JsonDocument** (.NET base class library, `System.Text.Json`) - parses the configuration file.
- **Environment** (.NET base class library) - expands environment variables and sets the exit code.
- **Process** and **ProcessStartInfo** (.NET base class library) - start the native program and wait for it.

Program depends on no other unit, subsystem, OTS item, or Shared Package.

### Callers

N/A - entry point, called by the .NET host (`dotnet`) as configured in `DotnetToolSettings.xml`. Within the
system, `Main` is the only caller of `GetTarget()`, which calls the other target methods.
