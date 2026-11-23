# DotnetToolWrapper

## Verification Approach

The system is verified by end-to-end system tests in `DotnetToolWrapperTests.cs`. Each test writes a
`DotnetToolWrapper.json` configuration file beside the built wrapper DLL in the test output folder, runs the
wrapper as a real process with `dotnet DemaConsulting.DotnetToolWrapper.dll ...` through the `Runner` helper, and
asserts on the exit code and the combined standard output and standard error. The tests treat the wrapper as a
black box: nothing is mocked, and the wrapper reads its configuration, resolves the program path, and starts the
native program exactly as it does inside an installed tool package.

The configured native program is the shell of the host operating system (`cmd.exe` from `COMSPEC` on Windows,
`/bin/sh` elsewhere), a small script that the test writes, or the `DemaConsulting.DotnetToolWrapper.EchoArgs`
test helper run by the `dotnet` host, so that the tests need no prebuilt native executable and the same tests run
on every platform. The shell commands echo a known text, exit with a requested code, or print the working
directory, and the helper prints the count and the exact text of each argument it received, which lets each test
observe what the native program received. The helper is needed for the exact argument check because a shell
re-parses its command line and cannot show how the arguments were split.

The tests key the configuration entry by the host target derived independently of the wrapper (the `HostTarget`
test helper uses the `OperatingSystem` checks and the lower-case name of `RuntimeInformation.OSArchitecture`),
so a wrong target name in the wrapper makes the tests fail instead of matching itself.

The platform requirements are verified by running the same system tests on each supported operating system,
processor architecture, and .NET runtime. Each test-result file is named `{os}_{asm}_{tfm}_{arch}.trx`, and the
platform requirements select their evidence with ReqStream source filters on these names (for example
`windows@`, `arm64@`, or `net8.0@`).

## Test Environment

- **Test framework**: xUnit v3 on Microsoft.Testing.Platform, run by `build.ps1` with `dotnet test`.
- **Target frameworks**: the test project targets `net8.0`, `net9.0`, `net10.0`, and `net11.0`, and runs once per
  framework; the matching .NET runtimes must be installed.
- **CI runners**: `ubuntu-latest` and `windows-latest` (x64), and `ubuntu-24.04-arm`, `windows-11-arm`, and
  `macos-latest` (arm64).
- **Files**: the tests write `DotnetToolWrapper.json` and, for the relative-path and multiple-target tests, a script
  folder beside the wrapper DLL in the test output folder, and create a unique folder under the temporary folder.
  Each test removes these files and folders when it ends.
- **Serialization**: the system tests share the configuration file beside the wrapper DLL, so they run in the
  `Sequential` test collection, which disables parallel execution.
- **Helpers**: the `DemaConsulting.DotnetToolWrapper.EchoArgs` console project is referenced by the test project,
  so the build copies the helper DLL beside the test assembly for every target framework; the exact argument test
  runs it with the `dotnet` host that runs the tests.
- **Environment**: the environment-variable test sets `DOTNETTOOLWRAPPER_TEST_SHELL_DIRECTORY` in the test
  process, so the wrapper process inherits it, and clears it when it ends.

## Acceptance Criteria

- All system tests pass with zero failures on every CI runner and every target framework.
- The wrapper exits with the exit code of the configured program, and that program receives the caller's
  arguments and working directory.
- Each configuration error (missing file, unsupported target, entry without a program) produces its message and
  exit code 1.
- ReqStream finds passing evidence for every system and platform requirement in the source-filtered test-result
  files.

## Test Scenarios

**DotnetToolWrapper_Arguments_Provided_PassesArgumentsToProgram**: the configured program is the shell, and the
wrapper is run with the shell arguments that echo `HelloWorld`. The wrapper exits with 0 and the output contains
`HelloWorld`, which proves that the wrapper found the entry for the host target, started the program, and passed
the arguments through unchanged. Run on every runner and framework, it is also the platform evidence.

**DotnetToolWrapper_Arguments_SpecialCharacters_PassesEachArgumentExactly**: the configured program is the `dotnet`
host, and the wrapper is run with the echo helper DLL followed by arguments that a careless pass-through would
change: a plain word, an argument with a space, an empty argument, arguments with embedded and surrounding double
quotes, an argument with backslashes (one at the end), and an argument with leading and trailing spaces. The
wrapper exits with 0, and the helper output is exactly the argument count and each argument unchanged and in
order, which proves that no argument is split, joined, dropped, or unquoted.

**DotnetToolWrapper_Target_MultipleTargets_RunsHostTargetProgram**: the configuration holds the host target, whose
program is a script that prints a marker, between decoy entries for other real targets whose program does not
exist. The wrapper exits with 0 and the output contains the marker, which proves that the wrapper selected the
entry of the host target and not another entry.

**DotnetToolWrapper_ExitCode_ProgramExits_ReturnsProgramExitCode**: the configured shell exits with the code under
test, with the values 0, 1, 42, and 255 (the boundary of a portable exit code). The wrapper exits with the same
code each time. Run on every runner and framework, it is also the platform evidence.

**DotnetToolWrapper_ProgramPath_Relative_ResolvesFromWrapperFolder**: a script is written to a unique folder beside
the wrapper DLL and configured with a path relative to the configuration file. The wrapper is run from a different
working directory, exits with 0, and the output contains the text printed by the script, which proves that the
relative path was resolved against the folder of the configuration file and not the working directory.

**DotnetToolWrapper_ProgramPath_EnvironmentVariable_ExpandsVariable**: the program path is configured as
`%DOTNETTOOLWRAPPER_TEST_SHELL_DIRECTORY%/{shell}` with the variable set to the folder of the shell. The wrapper
exits with 0 and the output contains the echoed text, which proves that the variable was expanded.

**DotnetToolWrapper_WorkingDirectory_Caller_RunsProgramInCallerDirectory**: the wrapper is run from a unique
temporary folder with the shell arguments that print the working directory. The output contains the name of that
folder, which proves that the program ran in the caller's working directory.

**DotnetToolWrapper_Configuration_FileMissing_ReportsErrorAndExitsWithOne**: the wrapper is run with no
configuration file beside it. It exits with 1 and the output contains `Missing configuration file` and
`DotnetToolWrapper.json`.

**DotnetToolWrapper_Target_NotInConfiguration_ReportsErrorAndExitsWithOne**: the configuration holds only the
entry `fake-target`, so no entry matches the host. The wrapper exits with 1 and the output contains
`does not support` and the name of the host target.

**DotnetToolWrapper_Configuration_ProgramMissing_ReportsErrorAndExitsWithOne**: the entry for the host target has a
`notProgram` property instead of `program`. The wrapper exits with 1 and the output contains
`Bad configuration`.
