package DotnetToolWrapper_Program {
    private import SoftwareItemMetadata::*;

    part def Program {
        doc
        /*
         * Entry point that maps the host operating system and architecture to a target name, reads the
         * program for that target from DotnetToolWrapper.json, runs it with the caller's arguments and
         * working directory, and exits with its exit code.
         */

        metadata info : SoftwareInfo {
            sourceFiles = ("src/DemaConsulting.DotnetToolWrapper/Program.cs");
            testFiles = ("test/DemaConsulting.DotnetToolWrapper.Tests/ProgramTests.cs");
            designFiles = ("docs/design/dotnet-tool-wrapper/program.md");
            verificationFiles = ("docs/verification/dotnet-tool-wrapper/program.md");
            requirementFiles = ("docs/reqstream/dotnet-tool-wrapper/program.yaml");
        }
    }
}
