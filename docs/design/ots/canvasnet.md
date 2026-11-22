## CanvasNet

CanvasNet (`DemaConsulting.CanvasNet.Tool`) is a .NET local tool that draws diagrams and figures from their
sources. It draws the pictures of the Pandoc documents during the document build, and the README banner.

### Purpose

The documents reference their diagram sources, such as the Mermaid run-time flowchart of the User Guide, rather
than committed images, so the source is the only artifact to maintain. CanvasNet is chosen because it draws SVG,
Mermaid, Vega-Lite, and OpenSCAD sources without a browser or other external runtime, ships as a .NET tool like
the rest of the toolchain, and provides a self-validation for qualification.

### Features Used

- **Pandoc filter** - the shim `docs/filters/canvasnet.lua`, listed in the `filters` of each document collection,
  starts `canvasnet pandoc-filter`, which replaces each picture that references a diagram source with the drawn
  image.
- **Mermaid drawing** - draws the run-time flowchart of the User Guide.
- **`render --strict`** - `docs/images/render-hero.ps1` draws the README banner `docs/images/hero.png` from
  `docs/images/hero.svg`, failing on unsupported constructs.
- **`compare`** - `render-hero.ps1 -Check` compares a fresh drawing with the committed banner.
- **Self-validation** - `--validate --results` writes the qualification evidence.

### Integration Pattern

CanvasNet is restored from the local tool manifest and started as `dotnet canvasnet`. The filter shim is written
by `dotnet canvasnet pandoc-filter --write-shim` and is never edited by hand; Pandoc runs it, and the drawn
pictures are cached under `docs/generated/canvasnet/`, which is never committed and is kept out of the documents
artifact. The CI documents job runs the self-validation before the first document that depends on the filter,
and its TRX file is the evidence for the CanvasNet requirements. The tool needs no other configuration and keeps
no state beyond the cache.
