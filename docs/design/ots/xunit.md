## xUnit

xUnit v3 (`xunit.v3`, with `xunit.runner.visualstudio`) is the test framework of the test project, running on
Microsoft.Testing.Platform. The Microsoft.Testing.Platform TRX report extension
(`Microsoft.Testing.Extensions.TrxReport`), referenced beside it, writes the test results; it is integrated and
qualified as part of this OTS item.

### Purpose

The requirements of DotnetToolWrapper are verified by automated unit and system tests whose results are the
compliance evidence. xUnit is chosen because it is a widely used, actively maintained .NET test framework that
supports data-driven tests and runs on Microsoft.Testing.Platform, which writes the TRX result files that
ReqStream reads.

### Features Used

- **Facts and theories** - `[Fact]` tests and data-driven `[Theory]` tests, discovered and run by `dotnet test`.
- **Multi-target runs** - the test project targets every framework of the wrapper, so each test runs once per
  framework.
- **TRX reports** - `--report-trx` (Microsoft.Testing.Extensions.TrxReport) writes one TRX file per framework,
  named `{os}_{asm}_{tfm}_{arch}.trx`, so the ReqStream source filters can select the evidence of each platform.

### Integration Pattern

xUnit is referenced as NuGet packages by `test/DemaConsulting.DotnetToolWrapper.Tests`, with the global usings
for `Xunit` in the project file. `build.ps1` builds the solution and runs `dotnet test` with the TRX report into
`artifacts/tests/`; the CI test job runs `build.ps1` on every operating system and architecture and uploads the
TRX files for ReqStream. xUnit has no self-validation; the passing results of the project's own tests in the TRX
files are the evidence for the xUnit requirements. The framework needs no initialization or disposal beyond the
test run.
