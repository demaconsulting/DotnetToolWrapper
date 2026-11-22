package DotnetToolWrapper {
    private import SoftwareItemMetadata::*;
    private import DotnetToolWrapper_Program::*;

    doc
    /*
     * DotnetToolWrapper is the managed entry point of a .NET Tool package that runs the native
     * executable configured for the host operating system and architecture.
     */

    part def DotnetToolWrapperSystem {
        doc /* The DotnetToolWrapper system as a whole. */

        metadata info : SoftwareInfo {
            testFiles = ("test/DemaConsulting.DotnetToolWrapper.Tests/DotnetToolWrapperTests.cs");
            designFiles = ("docs/design/dotnet-tool-wrapper.md");
            verificationFiles = ("docs/verification/dotnet-tool-wrapper.md");
            requirementFiles = (
                "docs/reqstream/dotnet-tool-wrapper.yaml",
                "docs/reqstream/dotnet-tool-wrapper/platform-requirements.yaml"
            );
        }

        part program : Program;
    }
}
