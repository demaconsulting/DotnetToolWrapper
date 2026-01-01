using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Integration tests for the DotnetToolWrapper application
/// </summary>
[TestClass]
public class IntegrationTests
{
    /// <summary>
    /// Test setup directory
    /// </summary>
    private string _testDirectory = string.Empty;

    /// <summary>
    /// Initialize test
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        // Create a unique test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), $"DotnetToolWrapperTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    /// <summary>
    /// Cleanup test
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
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
        var assemblyLocation = Path.GetDirectoryName(typeof(IntegrationTests).Assembly.Location);
        Assert.IsNotNull(assemblyLocation, "Assembly location should not be null");
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
            Assert.IsNotNull(directory, "DLL directory should not be null");
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
    [TestMethod]
    public void Main_WhenConfigFileMissing_ReturnsErrorWithMessage()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert
        Assert.AreEqual(1, exitCode, "Exit code should be 1 for missing config file");
        Assert.Contains("Missing configuration file", output, "Output should mention missing config file");
        Assert.Contains("DotnetToolWrapper.json", output, "Output should mention config file name");
    }

    /// <summary>
    /// Test that exit codes are properly passed through
    /// </summary>
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(42)]
    [DataRow(255)]
    public void Main_WithShellExitCode_ReturnsMatchingExitCode(int expectedExitCode)
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();
        CreateConfigFile(shellProgram);
        var shellArgs = GetExitCodeArgs(expectedExitCode);
        var args = new List<string> { dllPath };
        args.AddRange(shellArgs);

        // Act
        var exitCode = Runner.Run(out _, "dotnet", args.ToArray());

        // Assert
        Assert.AreEqual(expectedExitCode, exitCode, $"Exit code should be {expectedExitCode}");
    }

    /// <summary>
    /// Test that arguments are properly passed through
    /// </summary>
    [TestMethod]
    public void Main_WithShellArguments_PassesArgumentsToProgram()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();
        var testText = "HelloWorld";
        CreateConfigFile(shellProgram);
        var shellArgs = GetEchoArgs(testText);
        var args = new List<string> { dllPath };
        args.AddRange(shellArgs);

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", args.ToArray());

        // Assert
        Assert.AreEqual(0, exitCode, "Exit code should be 0");
        Assert.Contains(testText, output, $"Output should contain '{testText}'");
    }

    /// <summary>
    /// Test that unsupported target results in expected error
    /// </summary>
    [TestMethod]
    public void Main_WithUnsupportedTarget_ReturnsErrorWithMessage()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        Assert.IsNotNull(dllDirectory, "DLL directory should not be null");
        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { "fake-target", new { program = "fake" } }
        });
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert
        Assert.AreEqual(1, exitCode, "Exit code should be 1 for unsupported target");
        Assert.Contains("does not support", output, "Output should mention unsupported target");
    }

    /// <summary>
    /// Test that bad configuration results in expected error
    /// </summary>
    [TestMethod]
    public void Main_WithBadConfiguration_ReturnsErrorWithMessage()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        Assert.IsNotNull(dllDirectory, "DLL directory should not be null");
        var target = Program.GetTarget();
        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { target, new { notprogram = "fake" } }
        });
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert
        Assert.AreEqual(1, exitCode, "Exit code should be 1 for bad configuration");
        Assert.Contains("Bad configuration", output, "Output should mention bad configuration");
    }
}
