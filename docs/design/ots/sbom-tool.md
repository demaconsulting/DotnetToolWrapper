## SbomTool

The Microsoft SBOM tool (`Microsoft.Sbom.DotNetTool`) is a .NET local tool that generates software bill of
materials (SBOM) documents in the SPDX format.

### Purpose

Consumers of `DotnetToolWrapper.zip` need to know which files and components make up the wrapper. The Microsoft
SBOM tool is chosen to generate the base SPDX document because it scans a drop folder and the build components of a
project in one command, is maintained by Microsoft, and is distributed as a .NET tool. SpdxTool then completes and
validates the document.

### Features Used

- **SBOM generation** - `generate` writes `_manifest/spdx_2.2/manifest.spdx.json` and its `.sha256` hash file into
  the drop folder (`-b`).
- **File listing** - every file of the drop folder is listed with its checksums.
- **Component detection** - the project folder (`-bc`) is scanned for the packages used to build the wrapper.
- **Package identity** - the package name (`-pn`), version (`-pv`), supplier (`-ps`), and namespace base (`-nsb`)
  identify the wrapper in the document.

### Integration Pattern

The Microsoft SBOM tool is restored from the local tool manifest and started as `dotnet sbom-tool` in the CI build
job, after the wrapper files of each target framework are copied into the `drop` folder. The job runs
`dotnet sbom-tool generate -b drop -bc src/DemaConsulting.DotnetToolWrapper -pn DemaConsulting.DotnetToolWrapper
-pv {version} -ps DemaConsulting -nsb https://DemaConsulting.com/DotnetToolWrapper`, then
`dotnet fileassert --results artifacts/fileassert-sbom.trx sbom` checks the generated document before SpdxTool
changes it. The tool has no self-validation, so this FileAssert check is its qualification evidence. The generated
document is uploaded with the build artifacts. Each run is a single process with no state.
