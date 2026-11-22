## SbomTool

### Verification Approach

The Microsoft SBOM tool has no self-validation, so it is verified by a FileAssert check of the SPDX document it
generates. In the CI build job, `dotnet fileassert --results artifacts/fileassert-sbom.trx sbom` runs immediately
after `dotnet sbom-tool generate` and before SpdxTool changes the document, so the check sees the tool's own output.
The result file is uploaded with the build validation artifacts, and ReqStream reads it as the evidence for the
requirement. The generate step also fails the build when the tool reports an error.

### Test Scenarios

- **SbomTool_SpdxManifest**: `drop/_manifest/spdx_2.2/manifest.spdx.json` exists once with its `.sha256` hash file,
  is valid JSON with the `spdxVersion` and `documentNamespace` entries, at least one creator, at least four files,
  and at least one package, declares `SPDX-2.2`, names the `DemaConsulting.DotnetToolWrapper` package and the
  `https://DemaConsulting.com/DotnetToolWrapper` namespace, and lists `DemaConsulting.DotnetToolWrapper.dll` in
  the `net8.0`, `net9.0`, `net10.0`, and `net11.0` folders.
