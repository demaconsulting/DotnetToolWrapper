## ReqStream

ReqStream (`DemaConsulting.ReqStream`) is a .NET local tool that manages requirements in YAML files and traces them
to test results.

### Purpose

Continuous Compliance requires that every requirement is reachable from a root need and is satisfied by passing
tests before a release. ReqStream is chosen because it reads the requirements from YAML files kept with the code,
reads test results from TRX files, enforces the trace, and generates the requirements document, the
justifications, and the trace matrix.

### Features Used

- **Lint** - `--lint --enforce` reports structural problems and orphaned requirements in `lint.ps1`.
- **Trace and enforce** - `--tests "artifacts/**/*.trx" --enforce` fails the build when a requirement has no
  passing test evidence.
- **Source filters** - test names such as `windows@...` accept evidence only from TRX files whose names carry
  that platform, for the platform requirements.
- **Reports** - `--report`, `--justifications`, and `--matrix` write the Markdown that Pandoc turns into the
  Requirements Document and the Trace Matrix.
- **Self-validation** - `--validate --results` writes the qualification evidence.

### Integration Pattern

ReqStream is restored from the local tool manifest and reads `requirements.yaml`, which declares the root tags
and includes every file under `docs/reqstream/`. `lint.ps1` runs the lint on every change. In the CI documents
job ReqStream runs after all other evidence is collected: it runs its self-validation, then reads every TRX file
downloaded from the other jobs and written by the earlier document sections, writes the reports, and fails the
build with `--enforce` when a requirement is not satisfied. ReqStream keeps no state between runs.
