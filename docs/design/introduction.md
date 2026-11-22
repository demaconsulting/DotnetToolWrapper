# Introduction

DotnetToolWrapper is a small .NET console application that serves as the managed entry point of a .NET Tool
package wrapping a native program. When the tool runs, the wrapper maps the host operating system and processor
architecture to a target name, reads the native program configured for that target from `DotnetToolWrapper.json`,
runs it with the caller's arguments in the caller's working directory, and exits with its exit code. The software
is one system, **DotnetToolWrapper**, made of a single unit, **Program**. It has no subsystems and no run-time
dependencies beyond the .NET runtime. Off-the-shelf (OTS) tools build, test, check, and document it, but none of
them ships with the wrapper.

## Purpose

This document defines the design of each software item in DotnetToolWrapper: the architectural and detailed
design of the local items (the system and its unit), and the integration and usage design of the OTS software
items. It supports formal code review and Continuous Compliance: a reviewer should be able to understand how each
item satisfies its requirements without reading the source code. The document does not restate the requirements;
it explains how they are realized.

## Scope

Local items:

- **DotnetToolWrapper**: system design.
- **Program**: unit design.

OTS items:

- **CanvasNet**: integration and usage design.
- **FileAssert**: integration and usage design.
- **LintMark**: integration and usage design.
- **Pandoc**: integration and usage design.
- **ReqStream**: integration and usage design.
- **SpdxTool**: integration and usage design.
- **SysML2Tools**: integration and usage design.
- **TextLint**: integration and usage design.
- **WeasyPrint**: integration and usage design.
- **xUnit**: integration and usage design.

The following topics are out of scope:

- The test project and the build pipeline CI configuration; no design documents are produced for them.
- The internal design of the OTS items; only how this project integrates and uses each item is documented.
- How a package author builds a wrapped tool package; the *DotnetToolWrapper User Guide* covers packaging.

## Software Structure

The software structure is modeled in SysML2 under `docs/sysml2/` and rendered to the diagram below by
SysML2Tools in the build pipeline. AI agents should query the SysML2 model directly (see the `sysml2tools-query`
skill) rather than parsing this diagram or the prose elsewhere in this document. The model holds the run-time
composition of the shipped wrapper: the DotnetToolWrapper system and its Program unit. The OTS items are modeled
in the separate `OtsDependencies` package only so that their artifact locations can be queried; they are not
parts of the system, because none of them is shipped with the wrapper.

![Software structure of DotnetToolWrapper: the DotnetToolWrapper system containing the Program unit](SoftwareStructureView.svg)

## Folder Layout

- **src/** - source projects
  - **DemaConsulting.DotnetToolWrapper/** - wrapper console application (the Program unit)
- **test/** - test projects
  - **DemaConsulting.DotnetToolWrapper.Tests/** - unit tests of Program and system tests of the built wrapper
- **docs/** - documentation sources
  - **design/** - this Software Design Document
  - **reqstream/** - requirements files, traced to tests by ReqStream
  - **sysml2/** - SysML2 architecture model (common metadata, model, and design views)
  - **user_guide/** - DotnetToolWrapper User Guide
  - **verification/** - Software Verification Design Document

## Companion Artifact Structure

Each local software item has corresponding artifacts in parallel directory trees:

- Requirements: `docs/reqstream/{system-name}.yaml`, `docs/reqstream/{system-name}[/{subsystem-name}...]/{item}.yaml`
- Design: `docs/design/{system-name}.md`, `docs/design/{system-name}[/{subsystem-name}...]/{item}.md`
- Verification: `docs/verification/{system-name}.md`, `docs/verification/{system-name}[/{subsystem-name}...]/{item}.md`
- Source: `src/{SystemName}[/{SubsystemName}...]/{Item}.cs`
- Tests: `test/{SystemName}.Tests[/{SubsystemName}...]/{Item}Tests.cs`
- SysML2 model: `docs/sysml2/model/{system-name}.sysml`,
  `docs/sysml2/model/{system-name}[/{subsystem-name}...]/{item}.sysml`

For DotnetToolWrapper the local items map to these files:

- **DotnetToolWrapper** (system): requirements `docs/reqstream/dotnet-tool-wrapper.yaml` and
  `docs/reqstream/dotnet-tool-wrapper/platform-requirements.yaml`; design `docs/design/dotnet-tool-wrapper.md`;
  model `docs/sysml2/model/dotnet-tool-wrapper.sysml`; tests
  `test/DemaConsulting.DotnetToolWrapper.Tests/DotnetToolWrapperTests.cs`.
- **Program** (unit): requirements `docs/reqstream/dotnet-tool-wrapper/program.yaml`; design
  `docs/design/dotnet-tool-wrapper/program.md`; model `docs/sysml2/model/dotnet-tool-wrapper/program.sysml`;
  source `src/DemaConsulting.DotnetToolWrapper/Program.cs`; tests
  `test/DemaConsulting.DotnetToolWrapper.Tests/ProgramTests.cs`.

OTS items have integration/usage design documentation parallel to system folders, and are modeled in
`docs/sysml2/model/ots.sysml`:

- Requirements: `docs/reqstream/ots/{ots-name}.yaml`
- Design: `docs/design/ots/{ots-name}.md`
- Verification: `docs/verification/ots/{ots-name}.md`

Review-sets: defined in `.reviewmark.yaml`

## References

- *DotnetToolWrapper User Guide*
- *DotnetToolWrapper Requirements Document*
- [DotnetToolWrapper releases](https://github.com/demaconsulting/DotnetToolWrapper/releases)
- [.NET tools overview](https://learn.microsoft.com/dotnet/core/tools/global-tools)
