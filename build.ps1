# build.ps1
#
# PURPOSE:
#   Restores, builds (Release), and tests DemaConsulting.DotnetToolWrapper.slnx. The test project
#   targets net8.0, net9.0, net10.0, and net11.0, so 'dotnet test' runs the suite once per target
#   framework; the matching .NET runtimes must be installed. Tests use xUnit v3 on Microsoft.Testing.Platform
#   (selected in global.json); one TRX per framework goes to artifacts/tests (no merged TRX, so each test result
#   is reported once), named {os}_{asm}_{tfm}_{arch}.trx so that ReqStream source filters can select the
#   evidence of each operating system, framework, and architecture.
#
# CUSTOMIZATION:
#   This script belongs to the project: extend it as the project grows. Add steps at the
#   [PROJECT-SPECIFIC] points and add optional parameters that focus a run (for example
#   -Project <name> in a multi-library repository). Slow or environment-dependent test tiers
#   (GUI automation, integration) may be opt-in switches.
#   Run with no arguments it must still build and run the standard test suite (agents rely
#   on that), and never remove or weaken a test step to make it pass.

$solution = "DemaConsulting.DotnetToolWrapper.slnx"
$buildError = $false

Write-Host "Restoring dependencies..."
dotnet restore $solution
if ($LASTEXITCODE -ne 0) { $buildError = $true }

Write-Host "Building..."
dotnet build $solution --no-restore --configuration Release
if ($LASTEXITCODE -ne 0) { $buildError = $true }

Write-Host "Running tests..."
# Tag the test-result files with the host operating system so that ReqStream source filters
# (windows@, linux@, macos@) can tell the platforms apart; the test platform fills in {asm}, {tfm}, and {arch}.
$osTag = if ($IsWindows) { "windows" } elseif ($IsMacOS) { "macos" } elseif ($IsLinux) { "linux" } else { "unknown" }
dotnet test --solution $solution --no-build --configuration Release --report-trx `
    --report-trx-filename "${osTag}_{asm}_{tfm}_{arch}.trx" --no-artifact-post-processing --results-directory artifacts/tests
if ($LASTEXITCODE -ne 0) { $buildError = $true }

# [PROJECT-SPECIFIC] Add additional build steps here (e.g., packaging, publishing).

exit ($buildError ? 1 : 0)
