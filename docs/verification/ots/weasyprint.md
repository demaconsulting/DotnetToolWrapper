## WeasyPrint

### Verification Approach

WeasyPrint has no self-validation of the features used, so it is verified by FileAssert checks of the PDF/A
documents it renders. In the CI documents job, each HTML document is rendered to PDF by WeasyPrint and immediately
checked by the FileAssert test for that document: the PDF must exist, carry the expected title, author, and subject
metadata, have at least the expected number of pages, and contain the expected text. These checks run before
ReqStream, so their results are the evidence for the WeasyPrint requirement.

### Test Scenarios

- **WeasyPrint_DesignPdf**: the Software Design Document PDF has its metadata, at least 20 pages, and the expected
  text.
- **WeasyPrint_VerificationPdf**: the Software Verification Design Document PDF has its metadata, at least 10
  pages, and the expected text.
- **WeasyPrint_UserGuidePdf**: the User Guide PDF has its metadata, at least 10 pages, and the expected text.
