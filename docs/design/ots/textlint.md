## TextLint

TextLint (`DemaConsulting.TextLint.Tool`) is a .NET local tool that checks the spelling of text files.

### Purpose

Documentation, requirements, and source comments are review and compliance evidence, so spelling errors must be
caught before merge. TextLint is chosen because it checks the whole repository with a project dictionary of
technical terms, runs without external services, ships as a .NET tool like the rest of the toolchain, and
provides a self-validation for qualification.

### Features Used

- **Spell check** - `dotnet textlint` checks the files selected in `.textlint.yaml` and exits with a non-zero
  code on any finding.
- **Configuration** - `.textlint.yaml` holds the file globs, the ignored paths (such as `generated/`), the
  English language setting, and the technical terms.
- **Self-validation** - `--validate --results` writes the qualification evidence.

### Integration Pattern

TextLint is restored from the local tool manifest. `lint.ps1` runs it as one of the quality checks, and the CI
quality-checks job runs the self-validation before `lint.ps1` and uploads its TRX file for ReqStream. A
misspelling is fixed in the source; only genuine technical terms are added to the dictionary. TextLint reads only
the repository files and keeps no state between runs.
