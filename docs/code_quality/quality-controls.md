# Code Quality Controls

## Compiler and Analyzers

The project file of the wrapper, `src/DemaConsulting.DotnetToolWrapper/DemaConsulting.DotnetToolWrapper.csproj`,
enables these controls for every target framework (`net8.0`, `net9.0`, `net10.0`, and `net11.0`):

- **Warnings as errors** - `TreatWarningsAsErrors` turns every compiler and analyzer warning into a build error.
- **Nullable reference types** - `Nullable` is enabled, so possible null dereferences are reported.
- **.NET analyzers** - `EnableNETAnalyzers` with `AnalysisLevel` `latest` and the
  `Microsoft.CodeAnalysis.NetAnalyzers` package apply the latest .NET code-quality rules.
- **SonarAnalyzer** - the `SonarAnalyzer.CSharp` package applies the SonarSource C# rules during the build.
- **Code style** - `EnforceCodeStyleInBuild` reports violations of the `.editorconfig` code-style rules as build
  diagnostics.
- **Documentation** - `GenerateDocumentationFile` reports public members without XML documentation.

Because warnings are errors, a successful build of this version means that the compiler and the analyzers reported
no findings. The CI test job builds the solution on each of its five runners (Windows, Linux, and macOS, on x64 and
arm64).

## Formatting and Text Checks

The CI quality-checks job runs `lint.ps1`, which fails the build when any of these checks reports a problem:

- `dotnet format --verify-no-changes` checks that the C# code matches the `.editorconfig` formatting rules;
- TextLint checks the spelling of the repository text;
- LintMark checks the Markdown formatting;
- yamllint checks the YAML files;
- ReqStream, ReviewMark, VersionMark, and SysML2Tools check the requirements, review, version, and architecture
  model configuration; ReviewMark also fails the checks when a file requiring review is not covered by a review-set.

## CodeQL Code Scanning

The repository uses the GitHub CodeQL default setup for code scanning of the C# code and the GitHub Actions
workflows. GitHub runs the analysis on pushes and pull requests to the default branch and once a week, and the
results are kept in the code scanning alerts of the repository
(<https://github.com/demaconsulting/DotnetToolWrapper/security/code-scanning>), and the CodeQL checks are reported
on each pull request.

The build workflow does not run a second CodeQL analysis, because it would repeat the analysis of the default
setup.
