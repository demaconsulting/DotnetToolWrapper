using System.Diagnostics;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Tests of the Runner test helper
/// </summary>
public sealed class RunnerTests
{
    /// <summary>
    /// Validates that a program writing more than the pipe buffer holds runs to completion with all output captured
    /// </summary>
    [Fact]
    public void Runner_Run_LargeOutput_CapturesAllOutput()
    {
        // Arrange: ask the echo helper for over 1 MB of output (10,000 copies of a 2-line block of 100+ bytes)
        var echoArgsDll = GetEchoArgsDll();
        const int repeat = 10_000;
        var argument = new string('x', 100);

        // Act: run the helper; before the fix this deadlocked once the pipe buffer filled
        var exitCode = Runner.Run(out var output, "dotnet", echoArgsDll, $"--repeat={repeat}", argument);

        // Assert: the helper exits successfully and every block of output is captured
        Assert.Equal(0, exitCode);
        Assert.True(output.Length > 1_000_000, $"Expected over 1 MB of output, got {output.Length} characters");
        Assert.Equal(repeat, output.Split($"[{argument}]").Length - 1);
    }

    /// <summary>
    /// Validates that a program running longer than the timeout is killed promptly and has exited when the
    /// timeout exception is thrown
    /// </summary>
    [Fact]
    public void Runner_RunWithTimeout_LongRunningProgram_ThrowsTimeoutAndKillsProcess()
    {
        // Arrange: ask the echo helper to sleep far longer than the timeout
        var echoArgsDll = GetEchoArgsDll();
        var timeout = TimeSpan.FromSeconds(1);
        var stopwatch = Stopwatch.StartNew();

        // Act: run the helper with the short timeout
        var exception = Assert.Throws<TimeoutException>(
            () => Runner.RunWithTimeout(out _, timeout, "dotnet", echoArgsDll, "--sleep=60000", "arg"));
        stopwatch.Stop();

        // Assert: the timeout fires long before the helper would finish on its own
        Assert.True(
            stopwatch.Elapsed < TimeSpan.FromSeconds(30),
            $"Expected the timeout to fire promptly, took {stopwatch.Elapsed}");

        // Assert: the process ID is reported and the process is no longer running
        var processId = Assert.IsType<int>(exception.Data[Runner.ProcessIdDataKey]);
        Assert.False(IsRunning(processId), $"Expected process {processId} to have exited");
    }

    /// <summary>
    /// Validates that a repeat option whose value is not a count is echoed unchanged instead of being consumed
    /// </summary>
    [Fact]
    public void Runner_Run_InvalidRepeatOption_EchoesArgumentUnchanged()
    {
        // Arrange: pass a repeat option whose value does not parse as a count
        var echoArgsDll = GetEchoArgsDll();

        // Act: run the helper
        var exitCode = Runner.Run(out var output, "dotnet", echoArgsDll, "--repeat=abc", "arg");

        // Assert: both arguments are echoed once, unchanged
        Assert.Equal(0, exitCode);
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.TrimEnd('\r'));
        Assert.Equal(["count=2", "[--repeat=abc]", "[arg]"], lines);
    }

    /// <summary>
    /// Get the path of the echo helper DLL copied beside the test assembly
    /// </summary>
    /// <returns>Echo helper DLL path</returns>
    private static string GetEchoArgsDll()
    {
        var assemblyLocation = Path.GetDirectoryName(typeof(RunnerTests).Assembly.Location);
        Assert.NotNull(assemblyLocation);
        return Path.Combine(assemblyLocation, "DemaConsulting.DotnetToolWrapper.EchoArgs.dll");
    }

    /// <summary>
    /// Check whether a process with the specified ID is still running
    /// </summary>
    /// <param name="processId">Process ID</param>
    /// <returns>True when a running process has the ID</returns>
    private static bool IsRunning(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return !process.HasExited;
        }
        catch (ArgumentException)
        {
            // No process has the ID
            return false;
        }
        catch (InvalidOperationException)
        {
            // The process exited while it was being inspected
            return false;
        }
    }
}
