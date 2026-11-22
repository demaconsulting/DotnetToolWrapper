## BuildMark

BuildMark (`DemaConsulting.BuildMark`) is a .NET local tool that generates build notes from the Git history and
GitHub.

### Purpose

Each release must record its version and what changed since the previous release. BuildMark is chosen because it
derives the build notes from the Git tags and commits and from the GitHub issues and pull requests, so the notes
need no manual maintenance and match what was built.

### Features Used

- **Build report** - `--build-version {version} --report {file} --report-depth 1` writes the Markdown build notes:
  version information, changes, fixed bugs, known issues, and dependency updates.
- **Self-validation** - `--validate --results {file}` writes a TRX file of the built-in tests.

### Integration Pattern

BuildMark is restored from the local tool manifest and started as `dotnet buildmark` in the CI documents job, with
the `GH_TOKEN` environment variable set to the workflow token for the GitHub API. It writes
`docs/build_notes/generated/build_notes.md`, which Pandoc and WeasyPrint build into the Build Notes document. The
same Markdown file is copied to the `documents` artifact, and the release workflow uses it as the body of the GitHub
release. The self-validation runs in the same job, next to the document that depends on it. Each run is a single
process with no state.
