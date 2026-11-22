## FileAssert

### Verification Approach

FileAssert is verified by its self-validation. The CI documents job runs
`dotnet fileassert --validate --results artifacts/fileassert-self-validation.trx` after the document checks that
provide evidence for other OTS items and before ReqStream consumes that evidence. The software bill of materials
check in the CI build job also runs before ReqStream. Each check also fails the build when FileAssert cannot
evaluate it, so a passing build shows that FileAssert ran on the real documents and the software bill of materials.

### Test Scenarios

- **FileAssert_Results**: writes a TRX test-result file with passing and failing outcomes.
- **FileAssert_File**: checks that files matching a glob pattern exist in the expected number.
- **FileAssert_Text**: checks that a file contains the expected text.
- **FileAssert_Html**: checks HTML structure with an XPath query.
- **FileAssert_Xml**: checks XML structure with an XPath query.
- **FileAssert_Json**: checks JSON structure with a dot-notation query.
- **FileAssert_Pdf**: checks PDF metadata, page count, and body text.
