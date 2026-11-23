## Pandoc

Pandoc (`DemaConsulting.PandocTool`) is a .NET local tool that packages the Pandoc document converter.

### Purpose

The release documents are written as Markdown in document collections under `docs/` and must be delivered as one
HTML document each, which WeasyPrint then renders to PDF. Pandoc is chosen because it assembles several Markdown
files into one document with a table of contents, section numbers, an HTML template, and filters, and because the
tool package makes it available through `dotnet tool restore` without a separate installation.

### Features Used

- **Defaults files** - `--defaults docs/{collection}/definition.yaml` supplies the input files in reading order,
  the resource path, the template, the table of contents, section numbering, and the filters.
- **HTML template** - `docs/template/template.html` lays out the HTML that WeasyPrint renders.
- **Embedded resources** - `embed-resources` places the images, such as the SysML2 diagrams of this document, in
  the HTML file.
- **Lua filters** - the CanvasNet filter draws the diagram sources referenced by a document.
- **Metadata** - `--metadata version=...` and `--metadata date=...` stamp the build version and date, and the
  `title.txt` front matter supplies the title, author, and subject.

### Integration Pattern

Pandoc is restored from the local tool manifest and started as `dotnet pandoc` in the CI documents job, once per
document collection, writing `docs/{collection}/generated/{name}.html`. The SysML2 diagrams must be rendered into
`docs/design/generated/` before Pandoc builds this document, because that folder is on its resource path. Pandoc
has no self-validation of the features used, so the FileAssert checks of each generated HTML file (title,
embedded images, and expected text) are the evidence for the Pandoc requirement. Each run is a single process
with no state.
