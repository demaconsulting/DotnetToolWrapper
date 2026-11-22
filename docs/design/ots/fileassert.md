## FileAssert

FileAssert (`DemaConsulting.FileAssert`) is a .NET local tool that checks generated files against the assertions
in `.fileassert.yaml`.

### Purpose

Pandoc, WeasyPrint, and SysML2Tools can finish without an error and still produce a missing, empty, or incomplete
output, for example when a diagram is not found. FileAssert is chosen to catch this: it checks that each
generated diagram and document exists and holds its expected content, and it writes its results as TRX files, so
the checks are also the qualification evidence for the tools that produced the files.

### Features Used

- **File checks** - the expected file exists, a given number of times, with a minimum size.
- **XML checks** - XPath queries on the rendered SVG diagrams.
- **HTML checks** - XPath queries on the generated HTML, such as the title element and the embedded images.
- **PDF checks** - the title, author, and subject metadata, the minimum page count, and the text of the PDF.
- **Text checks** - expected text in the generated files.
- **Tags** - the tests are grouped by document (`design`, `user-guide`, `requirements`) and run one group at a
  time.
- **Results** - `--results` writes a TRX file; `--validate` runs the self-validation.

### Integration Pattern

FileAssert is restored from the local tool manifest and started as `dotnet fileassert`, with the tests defined in
`.fileassert.yaml` at the repository root. In the CI documents job each document section ends with
`dotnet fileassert --results artifacts/fileassert-{group}.trx {group}`. The `design` and `user-guide` groups run
before ReqStream, so their results are the evidence for the SysML2Tools, Pandoc, and WeasyPrint requirements; the
`requirements` group runs after ReqStream and checks only the final requirements documents. The self-validation
runs before ReqStream and is the evidence for the FileAssert requirement. Each run is a single process with no
state.
