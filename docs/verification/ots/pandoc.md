## Pandoc

### Verification Approach

Pandoc has no self-validation of the features used, so it is verified by FileAssert checks of the HTML it
generates. In the CI documents job, each document collection is converted to HTML by Pandoc and immediately checked
by the FileAssert test for that document: the HTML file must exist, have a title element, embed its expected
pictures where the document has any, and contain the expected text. These checks run before ReqStream, so their
results are the evidence for the Pandoc requirement.

### Test Scenarios

- **Pandoc_BuildNotesHtml**: the Build Notes HTML has a title and contains the version information and the tool
  versions.
- **Pandoc_CodeQualityHtml**: the Code Quality Report HTML has a title and contains `CodeQL`.
- **Pandoc_DesignHtml**: the Software Design Document HTML has a title, embeds the three SysML2 diagrams, and
  contains `Software Design Document`.
- **Pandoc_ReviewPlanHtml** and **Pandoc_ReviewReportHtml**: the Review Plan and Review Report HTML have a title
  and contain the document name and the review-set identifiers.
- **Pandoc_VerificationHtml**: the Software Verification Design Document HTML has a title and contains
  `Verification`.
- **Pandoc_UserGuideHtml**: the User Guide HTML has a title, embeds its flowchart, and contains `User Guide`.
