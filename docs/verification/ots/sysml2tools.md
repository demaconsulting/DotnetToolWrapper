## SysML2Tools

### Verification Approach

SysML2Tools is verified by its self-validation and by FileAssert checks of the diagrams it renders from the
model of this project:

- The CI documents job runs `dotnet sysml2tools --validate --results artifacts/sysml2tools-self-validation.trx`,
  which exercises `lint` and `render --format svg` on known model fixtures.
- `lint.ps1` runs `dotnet sysml2tools lint --strict-unresolved` on every file under `docs/sysml2/`, so a syntax
  error or an unresolved reference in the real model fails the quality checks.
- A missing diagram does not fail the Pandoc or WeasyPrint step; the document would silently lack the picture.
  The FileAssert test `SysML2Tools_DesignDiagramsSvg` therefore runs right after the render step and checks that
  each expected SVG file exists, has a non-trivial size, and has an `svg` root element.

### Test Scenarios

- **SysML2Tools_LintSelfTest**: lint passes a valid model fixture and reports an error for an invalid one.
- **SysML2Tools_RenderSvgSelfTest**: render writes a non-empty SVG file for the view of a model fixture.
- **SysML2Tools_DesignDiagramsSvg**: `SoftwareStructureView.svg` and `DotnetToolWrapperView.svg` were rendered
  from the model of this project into `docs/design/generated/`.
