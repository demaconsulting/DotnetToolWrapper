## VersionMark

### Verification Approach

VersionMark is verified by its self-validation. The CI documents job runs
`dotnet versionmark --validate --results artifacts/versionmark-self-validation.trx` before it publishes the tool
versions. The use of the tool on this project is also checked: `lint.ps1` fails on a problem in
`.versionmark.yaml`, and the FileAssert test `WeasyPrint_BuildNotesPdf` checks that the Build Notes document
contains the published tool versions.

### Test Scenarios

- **VersionMark_CapturesVersions**: the versions of configured tools are captured to a JSON file.
- **VersionMark_GeneratesMarkdownReport**: captured versions are published as a Markdown report.
- **VersionMark_LintPassesForValidConfig**: `--lint` passes a valid configuration.
- **VersionMark_LintReportsErrorsForInvalidConfig**: `--lint` reports the errors of a malformed configuration.
