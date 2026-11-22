## SpdxTool

### Verification Approach

SpdxTool is verified by its self-validation. The CI build job runs
`dotnet spdx-tool --validate --result artifacts/spdx-tool-self-validation.trx` before it runs
`spdx-workflow.yaml` on the software bill of materials of the wrapper. The workflow step fails the build when any
of its commands fails, including the NTIA minimum-elements validation.

### Test Scenarios

- **SpdxTool_RenameId**: renames an SPDX element identifier.
- **SpdxTool_RunNuGetWorkflow**: runs a workflow downloaded from a NuGet package, as used to query the .NET SDK.
- **SpdxTool_Query**: captures a value from the output of a program.
- **SpdxTool_AddPackage**: adds a package to an SPDX document.
- **SpdxTool_AddRelationship**: adds a relationship between SPDX elements.
- **SpdxTool_Hash**: updates the hash of an SPDX document.
- **SpdxTool_Ntia**: validates an SPDX document against the NTIA minimum elements.
- **SpdxTool_ToMarkdown**: writes a Markdown summary of an SPDX document.
