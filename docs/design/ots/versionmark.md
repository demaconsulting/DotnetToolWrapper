## VersionMark

VersionMark (`DemaConsulting.VersionMark`) is a .NET local tool that records the versions of the tools used in a
build.

### Purpose

Reproducing or investigating a build requires the versions of the SDK and the tools each CI job used. VersionMark
is chosen because it captures the versions from a checked-in configuration in each job, on every runner, and merges
the captures of all jobs into one Markdown report.

### Features Used

- **Configuration** - `.versionmark.yaml` names each tool, the command that prints its version, and the regular
  expression that extracts it.
- **Capture** - `--capture --job-id {id} --output {file} -- {tools}` writes the versions of the listed tools to a
  JSON file.
- **Publish** - `--publish --report {file} --report-depth 1 -- {pattern}` merges the JSON files of all jobs into a
  Markdown report.
- **Configuration lint** - `--lint` checks `.versionmark.yaml`.
- **Self-validation** - `--validate --results {file}` writes a TRX file of the built-in tests.

### Integration Pattern

VersionMark is restored from the local tool manifest and started as `dotnet versionmark`. `lint.ps1` runs `--lint`.
Each CI job (quality checks, each test runner, build, and documents) captures the versions of the tools it uses to
`artifacts/versionmark-{job-id}.json`, which is uploaded with the job's other evidence. The documents job runs the
self-validation and publishes all captures to `docs/build_notes/generated/versions.md`, which is part of the Build
Notes document. Each run is a single process with no state.
