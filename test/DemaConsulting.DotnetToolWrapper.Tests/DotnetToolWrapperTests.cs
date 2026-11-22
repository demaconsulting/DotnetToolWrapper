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
    /// Test setup directory
    /// </summary>
    private readonly string _testDirectory;

    /// <summary>
    /// Initialize test
    /// </summary>
    public DotnetToolWrapperTests()
    {
        // Create a unique test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DotnetToolWrapperTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
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

        var target = Program.GetTarget();

        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { target, new { program } }
        });

        File.WriteAllText(Path.Combine(directory, "DotnetToolWrapper.json"), json);
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
    /// Test that unsupported target results in expected error
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Target_NotInConfiguration_ReportsErrorAndExitsWithOne()
    {
        // Arrange: write a configuration that only contains a target that does not exist
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        Assert.NotNull(dllDirectory);
        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { "fake-target", new { program = "fake" } }
        });
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);

        // Act: run the wrapper with no arguments
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert: wrapper reports the unsupported target and exits with 1
        Assert.Equal(1, exitCode);
        Assert.Contains("does not support", output);
    }

    /// <summary>
    /// Test that bad configuration results in expected error
    /// </summary>
    [Fact]
    public void DotnetToolWrapper_Configuration_ProgramMissing_ReportsErrorAndExitsWithOne()
    {
        // Arrange: write a configuration for the current target that has no program entry
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        Assert.NotNull(dllDirectory);
        var target = Program.GetTarget();
        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { target, new { notProgram = "fake" } }
        });
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);

        // Act: run the wrapper with no arguments
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert: wrapper reports the bad configuration and exits with 1
        Assert.Equal(1, exitCode);
        Assert.Contains("Bad configuration", output);
    }
}
