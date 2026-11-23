# OTS Software Verification

This chapter describes how the off-the-shelf (OTS) software items of DotnetToolWrapper are verified. All of them
are tools used to build, test, check, and document the wrapper; none of them ships with it.

## Verification Strategy

Each OTS item is verified only for the behavior this project relies on, as stated in its requirements in
`docs/reqstream/ots/`. The evidence comes from one or more of these sources:

- **Self-validation**: the tool's `--validate` option runs its built-in tests and writes a TRX test-result file in
  the CI job that uses the tool.
- **Output checks**: FileAssert checks the files a tool generates (diagrams, HTML and PDF documents, and the SPDX
  document) for tools that have no self-validation of the behavior used.
- **Project tests**: the passing tests of the project are the evidence for the xUnit test framework.

ReqStream traces every OTS requirement to these results and fails the build with `--enforce` when one is not
satisfied. The same evidence is produced again on every pull request that upgrades a tool.

## Acceptance Criteria

An OTS item is accepted when every test listed by its requirements passes in the CI run of the build, and the
pipeline steps that use the item succeed.

## OTS Items

The following OTS items are described in the sections that follow:

- **BuildMark** - self-validation.
- **CanvasNet** - self-validation.
- **FileAssert** - self-validation.
- **LintMark** - self-validation.
- **Pandoc** - FileAssert checks of the generated HTML.
- **ReqStream** - self-validation.
- **ReviewMark** - self-validation.
- **SbomTool** - FileAssert check of the generated SPDX document.
- **SpdxTool** - self-validation.
- **SysML2Tools** - self-validation and FileAssert checks of the rendered diagrams.
- **TextLint** - self-validation.
- **VersionMark** - self-validation.
- **WeasyPrint** - FileAssert checks of the generated PDFs.
- **xUnit** - the passing project tests.
