## WeasyPrint

WeasyPrint (`DemaConsulting.WeasyPrintTool`) is a .NET local tool that packages the WeasyPrint HTML-to-PDF
renderer.

### Purpose

The release documents are delivered as PDF files attached to each GitHub release. WeasyPrint is chosen because it
renders the Pandoc HTML with its CSS template to archival PDF/A documents with the document metadata, and because
the tool package makes it available through `dotnet tool restore` without a separate installation.

### Features Used

- **HTML to PDF** - renders each HTML document produced by Pandoc, with its embedded images, to a PDF.
- **PDF/A** - `--pdf-variant pdf/a-3u` produces archival PDF/A-3u documents.
- **Metadata** - the HTML title, author, and description become the PDF title, author, and subject.

### Integration Pattern

WeasyPrint is restored from the local tool manifest and started as `dotnet weasyprint` in the CI documents job,
once per document, writing `docs/generated/DotnetToolWrapper {Document}.pdf`. The tool package ships only a
Windows x64 build, so the documents job runs on Windows. WeasyPrint has no self-validation of the features used,
so the FileAssert checks of each generated PDF (metadata, minimum page count, and expected text) are the evidence
for the WeasyPrint requirement. The documents job uploads the PDFs as the `documents` artifact, which the release
workflow attaches to the GitHub release. Each run is a single process with no state.
