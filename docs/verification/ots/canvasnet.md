## CanvasNet

### Verification Approach

CanvasNet is verified by its self-validation. The CI documents job runs
`dotnet canvasnet --validate --results artifacts/canvasnet-self-validation.trx` before its first Pandoc step,
because every document definition uses the CanvasNet Pandoc filter. The diagrams the filter draws are also
checked indirectly: the FileAssert check of the User Guide HTML expects its embedded picture.

### Test Scenarios

- **CanvasNetTool_PandocFilter**: the self-validation runs Pandoc with the CanvasNet filter on a document that
  references a diagram source and expects the reference to be replaced with the drawn picture.
- **CanvasNetTool_RenderMermaid**: the self-validation draws a Mermaid diagram and expects a picture file.
