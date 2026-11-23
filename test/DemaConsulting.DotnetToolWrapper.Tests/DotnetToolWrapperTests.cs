using System.Runtime.InteropServices;
using System.Text.Json;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// System-level integration tests that run the DotnetToolWrapper application via dotnet
/// </summary>
[Collection("Sequential")]
public sealed class DotnetToolWrapperTests : IDisposable
{
    /// <summary>
    /// Environment variable used by the environment-variable expansion test
    /// </summary>
    private const string ShellDirectoryVariable = "DOTNETTOOLWRAPPER_TEST_SHELL_DIRECTORY";

    /// <summary>
    /// Test setup directory
    /// </summary>
    private readonly string _testDirectory;

    /// <summary>
    /// Unique program folder beside the wrapper DLL, used by the relative-path test
    /// </summary>
    private readonly string _programDirectory;

    /// <summary>
    /// Initialize test
    /// </summary>
    public DotnetToolWrapperTests()
    {
        // Create a unique test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DotnetToolWrapperTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);

        // Name a unique program folder beside the wrapper DLL (created only by the tests that need it)
        var dllDirectory = Path.GetDirectoryName(GetDotnetToolWrapperDllPath());
        Assert.NotNull(dllDirectory);
        _programDirectory = Path.Combine(dllDirectory, Path.GetFileName(_testDirectory));
    }

    /// <summary>
    /// Cleanup test
    /// </summary>
    public void Dispose()
    {
        // Clean up test directory
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, true);
        }

        // Clean up program folder beside the DLL
        if (Directory.Exists(_programDirectory))
        {
            Directory.Delete(_programDirectory, true);
        }

        // Clean up environment variable
        Environment.SetEnvironmentVariable(ShellDirectoryVariable, null);

        // Clean up config file next to DLL
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        if (dllDirectory != null)
        {
            var configPath = Path.Combine(dllDirectory, "DotnetToolWrapper.json");
            if (File.Exists(configPath))
            {
                File.Delete(configPath);
            }
        }
    }

    /// <summary>
    /// Get the path to the DotnetToolWrapper DLL
    /// </summary>
    /// <returns>Path to DLL</returns>
    private static string GetDotnetToolWrapperDllPath()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(DotnetToolWrapperTests).Assembly.Location);
        Assert.NotNull(assemblyLocation);
        return Path.Combine(assemblyLocation, "DemaConsulting.DotnetToolWrapper.dll");
    }

    /// <summary>
    /// Create a DotnetToolWrapper.json configuration file
    /// </summary>
    /// <param name="program">Program to execute</param>
    /// <param name="directory">Directory for config file (defaults to DLL directory)</param>
    private static void CreateConfigFile(string program, string? directory = null)
    {
        // Default to DLL directory
        if (directory == null)
        {
            var dllPath = GetDotnetToolWrapperDllPath();
            directory = Path.GetDirectoryName(dllPath);
            Assert.NotNull(directory);
        }

        // Key the entry by the host target derived independently of the wrapper
        var target = HostTarget.ExpectedTarget();

        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { target, new { program } }
        });

        File.WriteAllText(Path.Combine(directory, "DotnetToolWrapper.json"), json);
    }

    /// <summary>
    /// Write a DotnetToolWrapper.json configuration file beside the wrapper DLL from raw entries
    /// </summary>
    /// <param name="entries">Configuration entries keyed by target name, in file order</param>
    private static void WriteConfigEntries(IEnumerable<KeyValuePair<string, object>> entries)
    {
        var dllDirectory = Path.GetDirectoryName(GetDotnetToolWrapperDllPath());
        Assert.NotNull(dllDirectory);
        var json = JsonSerializer.Serialize(new Dictionary<string, object>(entries));
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);
    }

    /// <summary>
    /// Get the path to the dotnet host that runs the tests
    /// </summary>
    /// <remarks>
    /// The runtime directory is {dotnet-root}/shared/Microsoft.NETCore.App/{version}/, so the host is three
    /// folders above it.
    /// </remarks>
    /// <returns>Full path to the dotnet executable</returns>
    private static string GetDotnetHostPath()
    {
        var dotnetRoot = Path.GetFullPath(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "..", "..", ".."));
        var dotnetHost = Path.Combine(dotnetRoot, OperatingSystem.IsWindows() ? "dotnet.exe" : "dotnet");
        Assert.True(File.Exists(dotnetHost), $"dotnet host not found at {dotnetHost}");
        return dotnetHost;
    }

    /// <summary>
    /// Get the path to the argument echo helper DLL copied beside the test assembly
    /// </summary>
    /// <returns>Path to the helper DLL</returns>
    private static string GetEchoArgsDllPath()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(DotnetToolWrapperTests).Assembly.Location);
        Assert.NotNull(assemblyLocation);
        var echoArgsDll = Path.Combine(assemblyLocation, "DemaConsulting.DotnetToolWrapper.EchoArgs.dll");
        Assert.True(File.Exists(echoArgsDll), $"Echo helper not found at {echoArgsDll}");
        return echoArgsDll;
    }

    /// <summary>
    /// Get the shell program name for the current OS
    /// </summary>
    /// <returns>Shell program name</returns>
    private static string GetShellProgram()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Use COMSPEC environment variable to get full path to cmd.exe
            var comspec = Environment.GetEnvironmentVariable("COMSPEC");
            return comspec ?? throw new InvalidOperationException("COMSPEC environment variable not found");
        }

        return "/bin/sh";
    }

    /// <summary>
    /// Get shell arguments for exit code test
    /// </summary>
    /// <param name="exitCode">Exit code to test</param>
    /// <returns>Shell arguments</returns>
    private static string[] GetExitCodeArgs(int exitCode)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? ["/c", $"exit {exitCode}"]
            : ["-c", $"exit {exitCode}"];
    }

    /// <summary>
    /// Get shell arguments for echo test
    /// </summary>
    /// <param name="text">Text to echo</param>
    /// <returns>Shell arguments</returns>
    private static string[] GetEchoArgs(string text)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? ["/c", $"echo {text}"]
            : ["-c", $"echo {text}"];
    }

    /// <summary>
    /// Get shell arguments that print the working directory
    /// </summary>
    /// <returns>Shell arguments</returns>
    private static string[] GetPrintWorkingDirectoryArgs()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? ["/c", "cd"]
            : ["-c", "pwd"];
    }

    /// <summary>
    /// Create a script that prints the specified text
    /// </summary>
    /// <param name="directory">Directory to create the script in</param>
    /// <param name="text">Text for the script to print</param>
    /// <returns>Script file name</returns>
    private static string CreateEchoScript(string directory, string text)
    {
        Directory.CreateDirectory(directory);

        // Windows runs a batch file directly
        if (OperatingSystem.IsWindows())
        {
            File.WriteAllText(Path.Combine(directory, "tool.cmd"), $"@echo {text}\r\n");
            return "tool.cmd";
        }

        // Other platforms run an executable shell script
        var scriptPath = Path.Combine(directory, "tool.sh");
        File.WriteAllText(scriptPath, $"#!/bin/sh\necho {text}\n");
        File.SetUnixFileMode(scriptPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        return "tool.sh";
    }

    /// <summary>
    /// Test that missing configuration file results in expected error
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Configuration_FileMissing_ReportsErrorAndExitsWithOne()
    {
        // Arrange: locate the wrapper DLL with no DotnetToolWrapper.json beside it
        var dllPath = GetDotnetToolWrapperDllPath();

        // Act: run the wrapper with no arguments
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert: wrapper reports the missing configuration file and exits with 1
        Assert.Equal(1, exitCode);
        Assert.Contains("Missing configuration file", output);
        Assert.Contains("DotnetToolWrapper.json", output);
    }

    /// <summary>
    /// Test that exit codes are properly passed through
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(255)]
    public void DotnetToolWrapper_ExitCode_ProgramExits_ReturnsProgramExitCode(int expectedExitCode)
    {
        // Arrange: configure the wrapper to run the shell, which exits with the requested code
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();
        CreateConfigFile(shellProgram);
        var shellArgs = GetExitCodeArgs(expectedExitCode);
        var args = new List<string> { dllPath };
        args.AddRange(shellArgs);

        // Act: run the wrapper with the shell exit arguments
        var exitCode = Runner.Run(out _, "dotnet", args.ToArray());

        // Assert: wrapper exits with the exit code of the wrapped program
        Assert.Equal(expectedExitCode, exitCode);
    }

    /// <summary>
    /// Test that arguments are properly passed through
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Arguments_Provided_PassesArgumentsToProgram()
    {
        // Arrange: configure the wrapper to run the shell, which echoes a known text
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();
        var testText = "HelloWorld";
        CreateConfigFile(shellProgram);
        var shellArgs = GetEchoArgs(testText);
        var args = new List<string> { dllPath };
        args.AddRange(shellArgs);

        // Act: run the wrapper with the shell echo arguments
        var exitCode = Runner.Run(out var output, "dotnet", args.ToArray());

        // Assert: wrapped program received the arguments and printed the text
        Assert.Equal(0, exitCode);
        Assert.Contains(testText, output);
    }

    /// <summary>
    /// Test that each argument reaches the program exactly, without splitting, joining, or unquoting
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Arguments_SpecialCharacters_PassesEachArgumentExactly()
    {
        // Arrange: configure the wrapper to run the echo helper through the dotnet host, with arguments that a
        // careless pass-through would split, drop, or unquote
        var dllPath = GetDotnetToolWrapperDllPath();
        CreateConfigFile(GetDotnetHostPath());
        string[] testArgs =
        [
            "plain",
            "with space",
            string.Empty,
            "quote\"inside",
            "\"quoted\"",
            "back\\slash\\",
            " leading and trailing "
        ];
        var args = new List<string> { dllPath, GetEchoArgsDllPath() };
        args.AddRange(testArgs);

        // Act: run the wrapper with the helper and the test arguments
        var exitCode = Runner.Run(out var output, "dotnet", args.ToArray());

        // Assert: the helper printed the same number of arguments, each one unchanged and in order
        Assert.Equal(0, exitCode);
        var expectedLines = new List<string> { $"count={testArgs.Length}" };
        expectedLines.AddRange(testArgs.Select(arg => $"[{arg}]"));
        var actualLines = output.ReplaceLineEndings("\n").TrimEnd('\n').Split('\n');
        Assert.Equal(expectedLines, actualLines);
    }

    /// <summary>
    /// Test that unsupported target results in expected error
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Target_NotInConfiguration_ReportsErrorAndExitsWithOne()
    {
        // Arrange: write a configuration that only contains a target that does not exist
        var dllPath = GetDotnetToolWrapperDllPath();
        WriteConfigEntries([new("fake-target", new { program = "fake" })]);

        // Act: run the wrapper with no arguments
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert: wrapper reports the unsupported host target by name and exits with 1
        Assert.Equal(1, exitCode);
        Assert.Contains("does not support", output);
        Assert.Contains(HostTarget.ExpectedTarget(), output);
    }

    /// <summary>
    /// Test that the program of the host target is run when the configuration holds other targets too
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Target_MultipleTargets_RunsHostTargetProgram()
    {
        // Arrange: configure decoy targets (before and after the host entry) whose program does not exist, and the
        // host target with a script that prints a marker
        var dllPath = GetDotnetToolWrapperDllPath();
        var hostTarget = HostTarget.ExpectedTarget();
        var marker = "HostTargetProgramRan";
        var scriptName = CreateEchoScript(_programDirectory, marker);
        var decoyProgram = new { program = "missing-decoy-program" };
        var decoyTargets = new[] { "win-x64", "linux-x64", "osx-arm64", "freebsd-x64", "linux-arm64" }
            .Where(target => target != hostTarget)
            .ToArray();
        var entries = new List<KeyValuePair<string, object>>();
        entries.AddRange(decoyTargets.Take(2).Select(target => new KeyValuePair<string, object>(target, decoyProgram)));
        entries.Add(new(hostTarget, new { program = $"{Path.GetFileName(_programDirectory)}/{scriptName}" }));
        entries.AddRange(decoyTargets.Skip(2).Select(target => new KeyValuePair<string, object>(target, decoyProgram)));
        WriteConfigEntries(entries);

        // Act: run the wrapper with no arguments
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert: the host target's script ran, not a decoy program
        Assert.Equal(0, exitCode);
        Assert.Contains(marker, output);
    }

    /// <summary>
    /// Test that bad configuration results in expected error
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Configuration_ProgramMissing_ReportsErrorAndExitsWithOne()
    {
        // Arrange: write a configuration for the host target that has no program entry
        var dllPath = GetDotnetToolWrapperDllPath();
        WriteConfigEntries([new(HostTarget.ExpectedTarget(), new { notProgram = "fake" })]);

        // Act: run the wrapper with no arguments
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert: wrapper reports the bad configuration and exits with 1
        Assert.Equal(1, exitCode);
        Assert.Contains("Bad configuration", output);
    }

    /// <summary>
    /// Test that a relative program path is resolved from the wrapper folder
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_ProgramPath_Relative_ResolvesFromWrapperFolder()
    {
        // Arrange: place a script in a sub-folder beside the wrapper and configure it by relative path
        var dllPath = GetDotnetToolWrapperDllPath();
        var marker = "RelativeProgramRan";
        var scriptName = CreateEchoScript(_programDirectory, marker);
        CreateConfigFile($"{Path.GetFileName(_programDirectory)}/{scriptName}");

        // Act: run the wrapper from a different working directory
        var exitCode = Runner.RunInDirectory(out var output, _testDirectory, "dotnet", dllPath);

        // Assert: the script beside the wrapper ran
        Assert.Equal(0, exitCode);
        Assert.Contains(marker, output);
    }

    /// <summary>
    /// Test that environment variables in the program path are expanded
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_ProgramPath_EnvironmentVariable_ExpandsVariable()
    {
        // Arrange: name the shell folder through an environment variable in the configured program path
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();
        var shellDirectory = Path.GetDirectoryName(shellProgram);
        Assert.NotNull(shellDirectory);
        Environment.SetEnvironmentVariable(ShellDirectoryVariable, shellDirectory);
        CreateConfigFile($"%{ShellDirectoryVariable}%/{Path.GetFileName(shellProgram)}");
        var testText = "HelloEnvironment";
        var args = new List<string> { dllPath };
        args.AddRange(GetEchoArgs(testText));

        // Act: run the wrapper with the shell echo arguments
        var exitCode = Runner.Run(out var output, "dotnet", args.ToArray());

        // Assert: the expanded program path found the shell, which printed the text
        Assert.Equal(0, exitCode);
        Assert.Contains(testText, output);
    }

    /// <summary>
    /// Test that the program runs in the working directory of the caller
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_WorkingDirectory_Caller_RunsProgramInCallerDirectory()
    {
        // Arrange: configure the wrapper to run the shell, which prints its working directory
        var dllPath = GetDotnetToolWrapperDllPath();
        CreateConfigFile(GetShellProgram());
        var args = new List<string> { dllPath };
        args.AddRange(GetPrintWorkingDirectoryArgs());

        // Act: run the wrapper from the unique test directory
        var exitCode = Runner.RunInDirectory(out var output, _testDirectory, "dotnet", args.ToArray());

        // Assert: the program printed the caller's working directory (compared by its unique folder name)
        Assert.Equal(0, exitCode);
        Assert.Contains(Path.GetFileName(_testDirectory), output);
    }
}
