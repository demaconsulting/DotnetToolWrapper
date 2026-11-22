## SpdxTool

SpdxTool (`DemaConsulting.SpdxTool`) is a .NET local tool that edits, validates, and summarizes SPDX software bill
of materials (SBOM) documents.

### Purpose

Consumers of `DotnetToolWrapper.zip` need to know which components and build tools went into the wrapper. The
Microsoft SBOM tool generates the base SPDX document; SpdxTool is chosen to complete it with the project's
package identity and the .NET SDK used for the build, to validate it against the NTIA minimum elements, and to
write a readable summary, all driven by one checked-in workflow file.

### Features Used

- **Workflows** - `run-workflow spdx-workflow.yaml` runs the steps below, including remote workflows that query
  the .NET SDK version and add it as a package.
- **Rename, add package, and add relationship** - give the root package its project identifier and record the
  .NET SDK.
- **Hash** - updates the hash of the SPDX document after the changes.
- **NTIA validation** - checks the document against the NTIA minimum elements.
- **Markdown summary** - writes `sbom_summary.md` beside the SPDX document.
- **Self-validation** - `--validate --result` writes the qualification evidence.

### Integration Pattern

SpdxTool is restored from the local tool manifest and started in the CI build job after the Microsoft SBOM tool
has generated `drop/_manifest/spdx_2.2/manifest.spdx.json`. The job runs the self-validation first and uploads
its TRX file for ReqStream. The workflow file `spdx-workflow.yaml` holds all parameters, so no other configuration
is needed; the enhanced SPDX document and its summary are uploaded with the build artifacts.
