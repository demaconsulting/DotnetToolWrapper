# Introduction

DotnetToolWrapper is a small .NET console application that serves as the managed entry point of a .NET Tool
package wrapping a native program. This document describes how each software item of DotnetToolWrapper is
verified.

## Purpose

This document defines the verification design of DotnetToolWrapper: for each software item it names the
verification approach, the test environment, the acceptance criteria, and the test scenarios, including normal
operation, boundary values, and error paths. It supports formal code review and Continuous Compliance: a reviewer
should be able to see how each requirement is proven without reading the test code. The document does not restate
the design; it explains how the design is shown to be correct. Requirement-to-test traceability is not repeated
here; ReqStream checks it on every build and publishes it in the *DotnetToolWrapper Trace Matrix*.

## Scope

Local items:

- **DotnetToolWrapper**: system verification.
- **Program**: unit verification.

OTS items:

- **BuildMark**: verification evidence.
- **CanvasNet**: verification evidence.
- **FileAssert**: verification evidence.
- **LintMark**: verification evidence.
- **Pandoc**: verification evidence.
- **ReqStream**: verification evidence.
- **ReviewMark**: verification evidence.
- **SbomTool**: verification evidence.
- **SpdxTool**: verification evidence.
- **SysML2Tools**: verification evidence.
- **TextLint**: verification evidence.
- **VersionMark**: verification evidence.
- **WeasyPrint**: verification evidence.
- **xUnit**: verification evidence.

The following topics are out of scope:

- The test project itself; it is the means of verification, not a subject of it.
- The build pipeline CI configuration; no verification documents are produced for it.
- The internal behavior of the OTS items; only the behavior this project relies on is verified.

## Folder Layout

- **test/** - test projects
  - **DemaConsulting.DotnetToolWrapper.Tests/** - one xUnit v3 test project
    - **DotnetToolWrapperTests.cs** - system tests that run the built wrapper with `dotnet`
    - **ProgramTests.cs** - unit tests of the Program unit
    - **Runner.cs** - helper that starts a process and captures its output and exit code, reading the output while
      the process runs and killing, then waiting for, a process that does not exit within its timeout (60 seconds
      unless a test passes a shorter one)
    - **RunnerTests.cs** - tests that the Runner helper captures output larger than the pipe buffer without blocking,
      kills a program that exceeds its timeout and waits for it to exit, and that the echo helper prints an
      option with an invalid value unchanged
    - **AssemblyInfo.cs** - the `Sequential` test collection that keeps the system tests from running in parallel
    - **HostTarget.cs** - helper that names the host target independently of the Program unit
  - **DemaConsulting.DotnetToolWrapper.EchoArgs/** - console helper that prints each argument it receives, run by
    the exact argument pass-through system test; leading `--repeat={n}` and `--sleep={ms}` options print the
    output n times or wait ms milliseconds for the Runner tests, and are consumed only when their value is a
    non-negative integer

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/{system-name}.yaml`, `docs/reqstream/{system-name}[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/{system-name}.md`, `docs/design/{system-name}[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/{system-name}.md`, `docs/verification/{system-name}[/{subsystem-name}...]/{item}.md`
- Source: `src/{SystemName}[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/{SystemName}.Tests[/{SubsystemName}...]/{Item}Tests.cs`

For DotnetToolWrapper the local items map to these files:

- **DotnetToolWrapper** (system): requirements `docs/reqstream/dotnet-tool-wrapper.yaml` and
  `docs/reqstream/dotnet-tool-wrapper/platform-requirements.yaml`; verification
  `docs/verification/dotnet-tool-wrapper.md`; tests
  `test/DemaConsulting.DotnetToolWrapper.Tests/DotnetToolWrapperTests.cs`.
- **Program** (unit): requirements `docs/reqstream/dotnet-tool-wrapper/program.yaml`; verification
  `docs/verification/dotnet-tool-wrapper/program.md`; source `src/DemaConsulting.DotnetToolWrapper/Program.cs`;
  tests `test/DemaConsulting.DotnetToolWrapper.Tests/ProgramTests.cs`.

OTS items have parallel artifacts:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Review-sets: defined in `.reviewmark.yaml`

## References

- *DotnetToolWrapper Software Design Document*
- *DotnetToolWrapper Requirements Document*
- *DotnetToolWrapper Trace Matrix*
- [DotnetToolWrapper releases](https://github.com/demaconsulting/DotnetToolWrapper/releases)
