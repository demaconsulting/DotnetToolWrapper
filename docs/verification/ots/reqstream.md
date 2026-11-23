## ReqStream

### Verification Approach

ReqStream is verified by its self-validation. The CI documents job runs
`dotnet reqstream --validate --results artifacts/reqstream-self-validation.trx` before it runs ReqStream with
`--enforce` on the requirements and all collected test results. The enforcing run itself is also evidence of use:
it fails the build when any requirement of this project is not satisfied.

### Test Scenarios

- **ReqStream_RequirementsProcessing**: requirements files with includes are loaded and parsed.
- **ReqStream_TraceMatrix**: requirements are traced to TRX test results in a trace matrix.
- **ReqStream_ReportExport**: the requirements report is written as Markdown.
- **ReqStream_TagsFiltering**: requirements are filtered by tag.
- **ReqStream_EnforcementMode**: `--enforce` exits with a non-zero code when a requirement is not satisfied.
- **ReqStream_OrphanDetection**: a requirement not reachable from a root requirement is reported.
- **ReqStream_Lint**: `--lint` reports structural problems in a requirements file, as used by `lint.ps1`.
