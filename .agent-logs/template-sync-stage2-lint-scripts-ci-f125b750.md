# Template Sync Report

**Result**: SUCCEEDED
**Report**: `.agent-logs/template-sync-stage2-lint-scripts-ci-f125b750.md`
**Mode**: Sync (limited scope: Stage 2, lint tooling, project scripts, and CI)

References: local template `C:\work\DemaConsulting\BuildTools\Agents-template` (primary) and the migrated
sibling `C:\work\DemaConsulting\BuildTools\TemplateDotNetTool` (CI structure). Where the two differ, the
template is newer (for example TextLint 0.1.0-beta.5 and LintMark 0.1.0-beta.3 against beta.4 and beta.2, and a
`.yamllint.yaml` with no line-length limit), so the template versions were adopted.

## Files

### .textlint.yaml

- **Template**: `Agents-template/.textlint.yaml`
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Created from the template, with the header, `files`, `enableGlobDot`, and `ignorePaths` kept
  verbatim. The `words` list keeps the template words and adds the terms TextLint actually flags:
  - From `.cspell.json`: `appsettings`, `mytool`, `runtimeconfig`.
  - From this repository: `dotnettool` (sbom-tool package id), `ncipollo` (release action owner), and `winver`
    (release workflow input).
  - For the shared upstream files: `Allman`, `gmock`, `nestings`, `netstandard`, `shopt`,
    `unidirectionality`, `Werror`, `Wextra`. TemplateDotNetTool adds four of these the same way.

  The other `.cspell.json` words were dropped, either because TextLint's dictionaries already accept them
  (DemaConsulting, nuget, freebsd, wasm, SBOM, SPDX, and others) or because they are unused (`globalconfig`,
  `markdownlint`, `cspell`).
- **Unresolved Questions**: none

### .lintmark.yaml, .yamllint.yaml, .yamlfix.toml, pip-requirements.txt

- **Template**: same paths in `Agents-template`
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Created (copied verbatim)
- **Unresolved Questions**: none

### .versionmark.yaml

- **Template**: `Agents-template/.versionmark.yaml`
- **Missing sections**: whole file
- **Heading depth issues**: none
- **Action**: Reported. It was not added because lint does not need it yet. It is deferred to the VersionMark stage.
- **Unresolved Questions**: none

### .config/dotnet-tools.json

- **Template**: `Agents-template/.config/dotnet-tools.json`
- **Missing sections**: pandoctool, weasyprinttool, reqstream, buildmark, versionmark, reviewmark, fileassert,
  dictionarymark, sysml2tools.tool, canvasnet.tool, sarifmark, sonarmark, docdown.tool (later stages)
- **Heading depth issues**: none
- **Action**: Sections added. Added `demaconsulting.textlint.tool` 0.1.0-beta.5 and `demaconsulting.lintmark`
  0.1.0-beta.3, both with `rollForward: false` (template pins). Kept `microsoft.sbom.dotnettool` 4.1.13 and
  `demaconsulting.spdxtool` 2.10.1 unchanged.
- **Unresolved Questions**: none

### build.ps1

- **Template**: `Agents-template/build.ps1`
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Created. The TODO was replaced with a PURPOSE block. Restore, build, and test pass
  `DemaConsulting.DotnetToolWrapper.slnx` explicitly. Tests run on net8.0, net9.0, net10.0, and net11.0 (one TRX
  per framework in `artifacts/tests`).
- **Unresolved Questions**: none

### lint.ps1

- **Template**: `Agents-template/lint.ps1`
- **Missing sections**: the compliance-tools block (ReqStream, VersionMark, ReviewMark, SysML2Tools), omitted on
  purpose
- **Heading depth issues**: none
- **Action**: Created. The script runs yamllint (via `.venv`), TextLint, LintMark, and
  `dotnet format DemaConsulting.DotnetToolWrapper.slnx --verify-no-changes`. A short comment marks where the
  compliance tools go when they are adopted.
- **Unresolved Questions**: none

### fix.ps1

- **Template**: `Agents-template/fix.ps1`
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Created. The solution-discovery block was replaced with
  `dotnet format DemaConsulting.DotnetToolWrapper.slnx`. The script always exits 0.
- **Unresolved Questions**: none

### .cspell.json, .markdownlint.json, .markdownlintignore

- **Template**: not found (superseded by `.textlint.yaml` and `.lintmark.yaml`)
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Deleted. The shared upstream files that these configs excluded (`AGENTS.md`, `.github/agents/**`,
  `.github/standards/**`, `.github/skills/**`) are now linted. They pass LintMark unchanged, and pass TextLint
  with the eight words listed above. No ignores were added.
- **Unresolved Questions**: none

### .github/workflows/build_on_push.yaml

- **Template**: none (reference `TemplateDotNetTool/.github/workflows/build_on_push.yaml`)
- **Missing sections**: `SONAR_TOKEN` passthrough and `security-events`/`pull-requests` permissions (later stage)
- **Heading depth issues**: none
- **Action**: Rebuilt:
  - The file keeps its name and its workflow name `Build`.
  - The inline cspell/markdownlint `Quality Checks` job (Node 20) was removed.
  - Added `workflow_dispatch` and the weekly Monday 17:00 UTC schedule, following the sibling.
  - The reusable job is still named `Build Artifacts`, so the required status checks keep their names:
    - `Build Artifacts / Build Artifacts`
    - `Build Artifacts / Test (ubuntu-latest)`, `Build Artifacts / Test (windows-latest)`, and
      `Build Artifacts / Test (macos-latest)`
  - The `semver` and `winver` inputs are unchanged.
- **Unresolved Questions**: none

### .github/workflows/build.yaml

- **Template**: none (reference `TemplateDotNetTool/.github/workflows/build.yaml`)
- **Missing sections**: VersionMark capture, SonarScanner, CodeQL, integration-test, and build-docs jobs (later stages)
- **Heading depth issues**: none
- **Action**: Rebuilt with three jobs:
  - `Quality Checks` (ubuntu): setup-dotnet 8.x to 11.x, `dotnet tool restore`, setup-python 3.14, then
    `pwsh ./lint.ps1`.
  - `Test (os)`: needs quality-checks and uses the same OS matrix (ubuntu-latest, windows-latest, macos-latest).
    It sets up 8.x to 11.x, runs `./build.ps1`, and uploads the TRX files as `test-results-{os}`.
  - `Build Artifacts`: needs test and is unchanged apart from the explicit `.slnx`. It still builds with the
    Version, InformationalVersion, and AssemblyVersion properties, assembles the same `drop/net8.0..net11.0`
    file set, runs sbom-tool and `spdx-tool run-workflow spdx-workflow.yaml`, and uploads `build-artifacts`.

  The test job no longer passes the version properties. Its binaries are not shipped and no test reads them.
  No Node setup is needed: TextLint and LintMark are dotnet tools.
- **Unresolved Questions**: none

### .github/workflows/release.yaml

- **Template**: none (reference `TemplateDotNetTool/.github/workflows/release.yaml`)
- **Missing sections**: documents attachment and `bodyFile: build_notes.md` (later stage)
- **Heading depth issues**: none
- **Action**: Rebuilt (formatting only):
  - Added the `---` document start and job comments.
  - Re-indented `steps` and `options` to the yamllint rules and removed trailing whitespace.

  Inputs (`semver`, `winver`, and `publish` with none or release), permissions, the `DotnetToolWrapper.zip`
  assembly (`net8.0/`, `net9.0/`, `net10.0/`, `net11.0/`), and the release assets (the zip and the SPDX
  manifest) are unchanged.
- **Unresolved Questions**: none

### spdx-workflow.yaml, .github/dependabot.yml, .github/ISSUE_TEMPLATE/*.yml

- **Template**: none
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Reported. `fix.ps1` (yamlfix) made formatting-only fixes: it added the `---` document start,
  indented the `spdx-workflow.yaml` step sequence and comments, and collapsed the double blank lines. The YAML
  meaning is unchanged, and CI's Enhance SBOM step passed.
- **Unresolved Questions**: none

### ARCHITECTURE.md, SECURITY.md, CODE_OF_CONDUCT.md, docs/usage.md

- **Template**: none
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Reported. `fix.ps1` (LintMark) applied:
  - Compact tables in ARCHITECTURE.md and SECURITY.md.
  - `-` bullets in CODE_OF_CONDUCT.md.
  - Lines wrapped to 120 columns in ARCHITECTURE.md and docs/usage.md.

  README.md needed no changes and was not restructured.
- **Unresolved Questions**: none

### CONTRIBUTING.md

- **Template**: none
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Reported:
  - Prerequisites now list PowerShell 7 and Python 3.
  - The build section uses `pwsh ./build.ps1`.
  - The project-structure tree lists the new config files and scripts, without cspell or markdownlint.
  - The documentation standards reference `.lintmark.yaml` and `.textlint.yaml`.
  - "Testing Your Changes" uses `fix.ps1`, `lint.ps1`, and `build.ps1`.
- **Unresolved Questions**: none

### test/DemaConsulting.DotnetToolWrapper.Tests/IntegrationTests.cs

- **Template**: none
- **Missing sections**: none
- **Heading depth issues**: none
- **Action**: Reported. TextLint flagged `notprogram`, so the anonymous-type member that serves as a fake
  configuration key was renamed to `notProgram`. The test still checks that a configuration without `program`
  is rejected.
- **Unresolved Questions**: none

## Commits

- `0646433` Adopt template lint tooling and project scripts
- `5464671` Run CI through lint.ps1 and build.ps1
- `df4ea2f` Apply fix.ps1 formatting
- `a397fd3` Fix lint findings
- (this report) Add Stage 2 template-sync report

## Quality Gates

- `pwsh ./build.ps1` passed: 0 warnings and 0 errors, and 11/11 tests passed on each of net8.0, net9.0, net10.0,
  and net11.0 (four TRX files).
- `pwsh ./lint.ps1` exited 0: yamllint clean (no warnings), TextLint 0 issues in 52 files, LintMark clean, and
  dotnet format clean.
- Workflow YAML passes yamllint. The workflow logic was checked by reading and by the CI run below.

## CI Run Result

- Run 45060544714 (<https://github.com/demaconsulting/DotnetToolWrapper/actions/runs/45060544714>) concluded
  **success**: Quality Checks, Test (ubuntu-latest, windows-latest, macos-latest), and Build Artifacts all
  succeeded.
- The `build-artifacts` file set (15 files: 3 per framework folder and 3 SPDX files) was compared with the
  previous run, 45058495720, and is identical.

## Issues in Shared Upstream Files

- None block lint. LintMark passes them unchanged.
- TextLint flags genuine technical terms in `.github/standards/`:
  - `cpp-language.md`: Allman, Wextra, Werror
  - `cpp-testing.md`: gmock
  - `design-documentation.md`: netstandard
  - `reqstream-usage.md`: unidirectionality, nestings
  - `sysml2-modeling.md`: shopt

  These terms were added to the `.textlint.yaml` word list instead of being excluded, as TemplateDotNetTool does.
  Upstream could add them to the shared template word list.
- `AGENTS.md` (shared) says CI `lint.ps1` checks reqstream, versionmark, reviewmark, and sysml2tools. That is
  only true after the later stages. The file was not edited.

## What Later Stages Must Enable

- **ReqStream (Stage 4)**:
  - Add `demaconsulting.reqstream` 1.12.0 to the manifest.
  - Add `requirements.yaml` and `docs/reqstream/**`.
  - Add `dotnet reqstream --lint --enforce --requirements requirements.yaml` to `lint.ps1` (in the compliance
    block).
  - In CI, download the `test-results-*` TRX files (already uploaded) and run `reqstream --enforce` in the docs
    job.
- **Doc pipeline (Stage 5)**:
  - Add pandoctool 3.12.1, weasyprinttool 70.0.0, fileassert 0.5.1, and canvasnet.tool 0.2.0-beta.13.
  - Add `.fileassert.yaml`.
  - Add a `build-docs` job (needs test and build).
  - Attach `documents` to the release.
- **SysML2Tools (Stage 6)**:
  - Add `demaconsulting.sysml2tools.tool` 0.3.0-beta.4.
  - Add `if (Test-Path docs/sysml2) { dotnet sysml2tools lint --strict-unresolved 'docs/sysml2/**/*.sysml' }` to
    `lint.ps1`.
- **ReviewMark (Stage 7)**:
  - Add `demaconsulting.reviewmark` 1.4.0 and `.reviewmark.yaml`.
  - Add `dotnet reviewmark --lint` to `lint.ps1`.
  - Add the plan and report generation to build-docs.
- **VersionMark, BuildMark, and code quality (Stage 8)**:
  - Add versionmark 1.4.3 and buildmark 1.3.0, plus sarifmark, sonarmark, and dotnet-sonarscanner if adopted.
  - Add `.versionmark.yaml` and `dotnet versionmark --lint` to `lint.ps1`.
  - Add a VersionMark capture/validate step to Quality Checks and Test (`dotnet git versionmark textlint lintmark`).
  - Add a CodeQL job with `.github/codeql-config.yml` and `security-events: write` in the callers.
  - Pass `SONAR_TOKEN` through, and add release `bodyFile: build_notes.md`.
- **Repository settings (maintainer)**:
  - Add `Build Artifacts / Quality Checks` to the required status checks in the "Main Branch" ruleset.
  - Today Test needs Quality Checks, so a lint failure skips the required Test and Build checks. GitHub reports
    a skipped job as passing, so a lint failure would not block a merge. The old Quality Checks job had the same
    gap.

## Summary

- **Conformant**: 6 (`.lintmark.yaml`, `.yamllint.yaml`, `.yamlfix.toml`, `pip-requirements.txt` verbatim;
  `fix.ps1`, `lint.ps1` adapted per Project Scripts rules) | **Deviations**: 3 (`lint.ps1` compliance block
  deferred; `.config/dotnet-tools.json` partial; `.versionmark.yaml` deferred) | **Updated**: 20
