## ReviewMark

### Verification Approach

ReviewMark is verified by its self-validation. The CI documents job runs
`dotnet reviewmark --validate --results artifacts/reviewmark-self-validation.trx` before it generates the Review
Plan and the Review Report. The use of the tool on this project is also checked: `lint.ps1` fails on a
configuration problem in `.reviewmark.yaml` or a file needing review that no review-set covers, the CI documents job
fails on the same coverage problem when it generates the review plan with `--enforce`, and the FileAssert tests
`Pandoc_ReviewPlanHtml`, `WeasyPrint_ReviewPlanPdf`, `Pandoc_ReviewReportHtml`, and `WeasyPrint_ReviewReportPdf`
check the documents built from the generated plan and report. The review evidence is read from the `index.json` of
the `reviews` branch, where no reviews are recorded yet, so the review report is generated without `--enforce`;
missing or outdated reviews are shown in the report but do not fail the build until reviews are recorded and
enforcement of the report is enabled.

### Test Scenarios

- **ReviewMark_ReviewPlanGeneration**: `--plan` writes a non-empty review plan for a test configuration.
- **ReviewMark_ReviewReportGeneration**: `--report` writes a non-empty review report for a test configuration and
  evidence store.
- **ReviewMark_Lint**: `--lint` reports the structural problems of a test configuration.
- **ReviewMark_Enforce**: `--enforce` makes ReviewMark exit with a non-zero code when the generated document has
  issues.
