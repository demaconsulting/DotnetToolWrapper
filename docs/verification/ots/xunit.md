## xUnit

### Verification Approach

xUnit is verified by the project's own tests. `build.ps1` runs the test project with `dotnet test` on
Microsoft.Testing.Platform, which discovers and runs the xUnit v3 tests on each target framework and writes one TRX
test-result file per framework. The xUnit requirements list a fact and a theory from each test class; their passing
results in the TRX files show that xUnit discovered, executed, and recorded both kinds of tests. Any failure of
xUnit itself fails `build.ps1` and the CI test job.

### Test Scenarios

- **Program_GetOs_WindowsPlatform_ReturnsWin**: a fact of the unit tests is discovered, run, and recorded.
- **Program_GetArchitecture_UnsupportedArchitecture_ReturnsUnknown**: a theory of the unit tests runs once per data
  row, and each row is recorded.
- **DotnetToolWrapper_Configuration_FileMissing_ReportsErrorAndExitsWithOne**: a fact of the system tests in the
  `Sequential` collection is discovered, run, and recorded.
- **DotnetToolWrapper_ExitCode_ProgramExits_ReturnsProgramExitCode**: a theory of the system tests runs once per
  exit code, and each row is recorded.
