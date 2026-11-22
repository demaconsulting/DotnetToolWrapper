## BuildMark

### Verification Approach

BuildMark is verified by its self-validation. The CI documents job runs
`dotnet buildmark --validate --results artifacts/buildmark-self-validation.trx` before it generates the build notes.
The use of the tool on this project is also checked: the FileAssert tests `Pandoc_BuildNotesHtml` and
`WeasyPrint_BuildNotesPdf` check that the Build Notes document built from the generated report contains its
version information.

### Test Scenarios

- **BuildMark_MarkdownReportGeneration**: a Markdown build report is written from mock build data.
- **BuildMark_GitIntegration**: version tags and commits are read from a mock Git history.
- **BuildMark_IssueTracking**: mock issues and pull requests are fetched and processed.
- **BuildMark_KnownIssuesReporting**: open bugs are listed as known issues.
- **BuildMark_RulesRouting**: items are routed to report sections by their labels and types.
