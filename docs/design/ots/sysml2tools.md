## SysML2Tools

SysML2Tools (`DemaConsulting.SysML2Tools.Tool`) is a .NET local tool that lints, queries, and renders SysML v2
models.

### Purpose

The software structure of DotnetToolWrapper is modeled in SysML2 under `docs/sysml2/`, and the model is the
authoritative source of that structure. SysML2Tools is chosen because it checks the model for errors, renders its
views to the diagrams embedded in this document, and lets AI agents query the model, so the diagrams and the
model cannot drift apart. It ships as a .NET tool like the rest of the toolchain and provides a self-validation
for qualification.

### Features Used

- **Lint** - `dotnet sysml2tools lint --strict-unresolved 'docs/sysml2/**/*.sysml'` reports syntax errors and
  unresolved references. The quoted glob is expanded by the tool itself, so the command works the same in every
  shell.
- **Render** - `dotnet sysml2tools render --output docs/design/generated --format svg` with the quoted globs
  `'docs/sysml2/common/**/*.sysml'`, `'docs/sysml2/model/**/*.sysml'`, and
  `'docs/sysml2/views/design-views.sysml'` renders every view declared in the views file to an SVG named after
  the view, such as `SoftwareStructureView.svg`.
- **Query** - `dotnet sysml2tools query` answers structural questions, such as the purpose and the artifact
  locations of an item; AI agents use it through the `sysml2tools-query` skill.
- **Self-validation** - `--validate --results` writes the qualification evidence.

### Integration Pattern

SysML2Tools is restored from the local tool manifest and reads only the SysML2 files; it needs no configuration
file. `lint.ps1` lints the model on every change, which gates all later CI jobs. In the CI documents job the
design section runs the self-validation and then renders the views into `docs/design/generated/`, before Pandoc
builds this document. A missing SVG does not fail Pandoc or WeasyPrint, which would silently leave out the
diagram, so FileAssert checks that each expected SVG exists and is well-formed XML; that check and the
self-validation are the evidence for the SysML2Tools requirements. Each run is a single process with no state.
