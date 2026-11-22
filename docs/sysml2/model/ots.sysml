package OtsDependencies {
    private import SoftwareItemMetadata::*;

    doc
    /*
     * Off-the-shelf tools used to build, test, check, and document DotnetToolWrapper. None of them
     * is part of the shipped wrapper, so the system does not reference them as parts.
     */

    part def BuildMark {
        doc /* OTS: BuildMark - generates the build notes from the Git history and the GitHub issues and pull requests. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/buildmark.md");
            verificationFiles = ("docs/verification/ots/buildmark.md");
            requirementFiles = ("docs/reqstream/ots/buildmark.yaml");
        }
    }

    part def CanvasNet {
        doc /* OTS: CanvasNet - draws the diagrams referenced by the Pandoc documents and the README picture. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/canvasnet.md");
            verificationFiles = ("docs/verification/ots/canvasnet.md");
            requirementFiles = ("docs/reqstream/ots/canvasnet.yaml");
        }
    }

    part def FileAssert {
        doc /* OTS: FileAssert - checks that the generated diagrams and documents exist and hold the expected content. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/fileassert.md");
            verificationFiles = ("docs/verification/ots/fileassert.md");
            requirementFiles = ("docs/reqstream/ots/fileassert.yaml");
        }
    }

    part def LintMark {
        doc /* OTS: LintMark - checks and fixes the formatting of the Markdown files. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/lintmark.md");
            verificationFiles = ("docs/verification/ots/lintmark.md");
            requirementFiles = ("docs/reqstream/ots/lintmark.yaml");
        }
    }

    part def Pandoc {
        doc /* OTS: Pandoc - converts the Markdown sources of each document collection to HTML. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/pandoc.md");
            verificationFiles = ("docs/verification/ots/pandoc.md");
            requirementFiles = ("docs/reqstream/ots/pandoc.yaml");
        }
    }

    part def ReqStream {
        doc /* OTS: ReqStream - checks the requirements files and traces each requirement to passing tests. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/reqstream.md");
            verificationFiles = ("docs/verification/ots/reqstream.md");
            requirementFiles = ("docs/reqstream/ots/reqstream.yaml");
        }
    }

    part def ReviewMark {
        doc /* OTS: ReviewMark - checks the review configuration and generates the review plan and review report. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/reviewmark.md");
            verificationFiles = ("docs/verification/ots/reviewmark.md");
            requirementFiles = ("docs/reqstream/ots/reviewmark.yaml");
        }
    }

    part def SbomTool {
        doc /* OTS: Microsoft SBOM tool - generates the SPDX software bill of materials of the wrapper drop folder. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/sbom-tool.md");
            verificationFiles = ("docs/verification/ots/sbom-tool.md");
            requirementFiles = ("docs/reqstream/ots/sbom-tool.yaml");
        }
    }

    part def SpdxTool {
        doc /* OTS: SpdxTool - enhances, validates, and summarizes the software bill of materials. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/spdx-tool.md");
            verificationFiles = ("docs/verification/ots/spdx-tool.md");
            requirementFiles = ("docs/reqstream/ots/spdx-tool.yaml");
        }
    }

    part def SysML2Tools {
        doc /* OTS: SysML2Tools - lints, queries, and renders the SysML2 architecture model. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/sysml2tools.md");
            verificationFiles = ("docs/verification/ots/sysml2tools.md");
            requirementFiles = ("docs/reqstream/ots/sysml2tools.yaml");
        }
    }

    part def TextLint {
        doc /* OTS: TextLint - checks the spelling of the repository text. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/textlint.md");
            verificationFiles = ("docs/verification/ots/textlint.md");
            requirementFiles = ("docs/reqstream/ots/textlint.yaml");
        }
    }

    part def VersionMark {
        doc /* OTS: VersionMark - captures the versions of the build tools in each CI job and publishes them. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/versionmark.md");
            verificationFiles = ("docs/verification/ots/versionmark.md");
            requirementFiles = ("docs/reqstream/ots/versionmark.yaml");
        }
    }

    part def WeasyPrint {
        doc /* OTS: WeasyPrint - renders the HTML of each document collection to a PDF/A document. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/weasyprint.md");
            verificationFiles = ("docs/verification/ots/weasyprint.md");
            requirementFiles = ("docs/reqstream/ots/weasyprint.yaml");
        }
    }

    part def XUnit {
        doc /* OTS: xUnit - discovers and runs the automated tests and records their results. */

        metadata info : SoftwareInfo {
            designFiles = ("docs/design/ots/xunit.md");
            verificationFiles = ("docs/verification/ots/xunit.md");
            requirementFiles = ("docs/reqstream/ots/xunit.yaml");
        }
    }
}
