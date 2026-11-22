## ReviewMark

### Verification Approach

ReviewMark is verified by its self-validation. The CI documents job runs
`dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx` before it generates the Review
Plan and the Review Report. The use of the tool on this project is also checked: `lint.ps1` fails on a
configuration problem in `.reviewmark.yaml`, and the FileAssert tests `Pandoc_ReviewPlanHtml`,
`WeasyPrint_ReviewPlanPdf`, `Pandoc_ReviewReportHtml`, and `WeasyPrint_ReviewReportPdf` check the documents built
from the generated plan and report.

### Test Scenarios

- **ReviewMark_ReviewPlanGeneration**: `--plan` writes a non-empty review plan for a test configuration.
- **ReviewMark_ReviewReportGeneration**: `--report` writes a non-empty review report for a test configuration and
  evidence store.
- **ReviewMark_Lint**: `--lint` reports the structural problems of a test configuration.
- **ReviewMark_Elaborate**: `--elaborate` prints the identifier, fingerprint, and files of a named review-set.
