# OTS Software Integration Design

This chapter describes how DotnetToolWrapper selects, integrates, and qualifies its off-the-shelf (OTS) software
items. All of them are tools used to build, test, check, and document the wrapper; none of them is part of the
shipped wrapper, which depends only on the .NET runtime. The sections that follow cover the specifics of each OTS
item.

## Selection Criteria

OTS items are selected when they:

- have a license compatible with the MIT License of this project (MIT or another permissive license);
- are actively maintained and distributed through NuGet, so they install the same way on every build machine;
- provide qualification evidence: a self-validation that writes test results, or output that can be checked
  automatically;
- run on the operating systems used by the CI jobs that need them.

## Version Management Policy

Every OTS tool is pinned to an exact version in the local tool manifest `.config/dotnet-tools.json`, with
`rollForward` disabled where the tool supports it, and the xUnit packages are pinned in the package references of
the test project. A build therefore always uses the same tool versions, on a developer machine and in CI.
Upgrades are proposed by Dependabot or made by a maintainer, and go through the normal pull request review and
CI checks; an upgrade that changes the behavior a tool is relied on for also requires an update of its
integration design and requirements. Version numbers are recorded in the software bill of materials, not in this
document.

## General Integration Approach

All OTS items except xUnit are .NET local tools, restored with `dotnet tool restore` and started as
`dotnet {command}` from the repository scripts and the CI workflow:

- `build.ps1` builds the solution and runs the xUnit tests;
- `lint.ps1` runs TextLint, LintMark, ReqStream, ReviewMark, and SysML2Tools as quality checks, and `fix.ps1`
  runs the LintMark fixer;
- the CI build job runs SpdxTool on the software bill of materials;
- the CI documents job runs ReviewMark, SysML2Tools, Pandoc, CanvasNet, WeasyPrint, FileAssert, and ReqStream to
  build and check the release documents.

xUnit is a NuGet test framework referenced by the test project. Each tool runs as a separate process with its
configuration in a checked-in file or on its command line; none needs initialization or disposal beyond that
process. A tool that fails exits with a non-zero code, which fails the script or the CI step that started it.

## Qualification Strategy

Each OTS item has requirements in `docs/reqstream/ots/` that state the behavior the project relies on. The
evidence for those requirements is:

- the self-validation of the tool (`--validate`), which writes a TRX test-result file in the CI job that uses
  the tool;
- the FileAssert checks of the files a tool generates, for tools without a self-validation of the used
  behavior (Pandoc and WeasyPrint) and for the rendered design diagrams;
- the passing test suite for xUnit.

ReqStream traces every OTS requirement to these results and fails the build with `--enforce` when one is not
satisfied. Each OTS requirement is linked as a child of a repository quality requirement. Before an upgrade is
accepted, the CI run on the pull request must pass with the new version, which repeats the self-validations and
checks.

## OTS Items

The following OTS items are described in the sections that follow:

- **CanvasNet** - draws the diagrams of the Pandoc documents and the README picture.
- **FileAssert** - checks the generated diagrams and documents.
- **LintMark** - checks and fixes the Markdown formatting.
- **Pandoc** - converts the Markdown sources of each document to HTML.
- **ReqStream** - checks the requirements and traces them to test evidence.
- **ReviewMark** - checks the review configuration and generates the review plan and report.
- **SpdxTool** - enhances, validates, and summarizes the software bill of materials.
- **SysML2Tools** - lints, queries, and renders the SysML2 architecture model.
- **TextLint** - checks the spelling of the repository text.
- **WeasyPrint** - renders the HTML of each document to PDF/A.
- **xUnit** - runs the automated tests and records their results.
