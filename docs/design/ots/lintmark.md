## LintMark

LintMark (`DemaConsulting.LintMark`) is a .NET local tool that checks and fixes the formatting of Markdown files.

### Purpose

The Markdown files are documentation and compliance evidence, and several of them are assembled into PDF
documents, so they must follow one consistent format. LintMark is chosen because it checks and fixes the same
rules, ships as a .NET tool like the rest of the toolchain, and provides a self-validation for qualification.

### Features Used

- **Lint** - `dotnet lintmark` reports every violation of the rules configured in `.lintmark.yaml` and exits with
  a non-zero code.
- **Fix** - `dotnet lintmark --fix` rewrites fixable violations, such as over-long lines and list markers, to the
  canonical form.
- **Ignore list** - `.lintmark.yaml` excludes build output (`generated/`), the Python environment, and other
  folders that are not project sources.
- **Self-validation** - `--validate --results` writes the qualification evidence.

### Integration Pattern

LintMark is restored from the local tool manifest. `fix.ps1` runs `dotnet lintmark --fix` and `lint.ps1` runs
`dotnet lintmark`, which fails the quality checks on any finding. The CI quality-checks job runs the
self-validation before `lint.ps1` and uploads its TRX file for ReqStream. LintMark reads only the repository
files and its configuration file, and keeps no state between runs.
