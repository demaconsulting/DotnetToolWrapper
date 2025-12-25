using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Tests for the DotnetToolWrapper application
/// </summary>
[TestClass]
public class DotnetToolWrapperTests
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
        var assemblyLocation = Path.GetDirectoryName(typeof(DotnetToolWrapperTests).Assembly.Location);
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
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "/bin/sh";
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
    public void TestMissingConfigFile()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert
        Assert.AreEqual(1, exitCode, "Exit code should be 1 for missing config file");
        Assert.IsTrue(output.Contains("Missing configuration file"), "Output should mention missing config file");
        Assert.IsTrue(output.Contains("DotnetToolWrapper.json"), "Output should mention config file name");
    }

    /// <summary>
    /// Test that exit codes are properly passed through
    /// </summary>
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(42)]
    [DataRow(255)]
    public void TestExitCodes(int expectedExitCode)
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();

        // Create config file pointing to shell
        CreateConfigFile(shellProgram);

        // Get shell arguments to exit with specific code
        var shellArgs = GetExitCodeArgs(expectedExitCode);

        // Prepare arguments for dotnet command
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
    public void TestArgumentPassing()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var shellProgram = GetShellProgram();
        var testText = "HelloWorld";

        // Create config file pointing to shell
        CreateConfigFile(shellProgram);

        // Get shell arguments to echo text
        var shellArgs = GetEchoArgs(testText);

        // Prepare arguments for dotnet command
        var args = new List<string> { dllPath };
        args.AddRange(shellArgs);

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", args.ToArray());

        // Assert
        Assert.AreEqual(0, exitCode, "Exit code should be 0");
        Assert.IsTrue(output.Contains(testText), $"Output should contain '{testText}'");
    }

    /// <summary>
    /// Test that unsupported target results in expected error
    /// </summary>
    [TestMethod]
    public void TestUnsupportedTarget()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        Assert.IsNotNull(dllDirectory, "DLL directory should not be null");

        // Create config file with fake target
        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { "fake-target", new { program = "fake" } }
        });
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert
        Assert.AreEqual(1, exitCode, "Exit code should be 1 for unsupported target");
        Assert.IsTrue(output.Contains("does not support"), "Output should mention unsupported target");
    }

    /// <summary>
    /// Test that bad configuration results in expected error
    /// </summary>
    [TestMethod]
    public void TestBadConfiguration()
    {
        // Arrange
        var dllPath = GetDotnetToolWrapperDllPath();
        var dllDirectory = Path.GetDirectoryName(dllPath);
        Assert.IsNotNull(dllDirectory, "DLL directory should not be null");
        var target = Program.GetTarget();

        // Create config file without program property
        var json = JsonSerializer.Serialize(new Dictionary<string, object>
        {
            { target, new { notprogram = "fake" } }
        });
        File.WriteAllText(Path.Combine(dllDirectory, "DotnetToolWrapper.json"), json);

        // Act
        var exitCode = Runner.Run(out var output, "dotnet", dllPath);

        // Assert
        Assert.AreEqual(1, exitCode, "Exit code should be 1 for bad configuration");
        Assert.IsTrue(output.Contains("Bad configuration"), "Output should mention bad configuration");
    }

    /// <summary>
    /// Test GetOs method returns valid OS identifier
    /// </summary>
    [TestMethod]
    public void TestGetOs()
    {
        // Act
        var os = Program.GetOs();

        // Assert
        Assert.IsNotNull(os);
        Assert.IsTrue(
            os is "win" or "linux" or "freebsd" or "osx" or "unknown",
            $"OS should be one of the known values, got: {os}");
    }

    /// <summary>
    /// Test GetArchitecture method returns valid architecture identifier
    /// </summary>
    [TestMethod]
    public void TestGetArchitecture()
    {
        // Act
        var arch = Program.GetArchitecture();

        // Assert
        Assert.IsNotNull(arch);
        Assert.IsTrue(
            arch is "x86" or "x64" or "arm" or "arm64" or "wasm" or "s390x" or "unknown",
            $"Architecture should be one of the known values, got: {arch}");
    }

    /// <summary>
    /// Test GetTarget method returns valid target string
    /// </summary>
    [TestMethod]
    public void TestGetTarget()
    {
        // Act
        var target = Program.GetTarget();

        // Assert
        Assert.IsNotNull(target);
        Assert.IsTrue(target.Contains('-'), "Target should contain a hyphen");
        
        var parts = target.Split('-');
        Assert.AreEqual(2, parts.Length, "Target should have exactly two parts");
    }
}
