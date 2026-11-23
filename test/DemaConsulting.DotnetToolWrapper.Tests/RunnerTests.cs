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
        var assemblyLocation = Path.GetDirectoryName(typeof(RunnerTests).Assembly.Location);
        Assert.NotNull(assemblyLocation);
        var echoArgsDll = Path.Combine(assemblyLocation, "DemaConsulting.DotnetToolWrapper.EchoArgs.dll");
        const int repeat = 10_000;
        var argument = new string('x', 100);

        // Act: run the helper; before the fix this deadlocked once the pipe buffer filled
        var exitCode = Runner.Run(out var output, "dotnet", echoArgsDll, $"--repeat={repeat}", argument);

        // Assert: the helper exits successfully and every block of output is captured
        Assert.Equal(0, exitCode);
        Assert.True(output.Length > 1_000_000, $"Expected over 1 MB of output, got {output.Length} characters");
        Assert.Equal(repeat, output.Split($"[{argument}]").Length - 1);
    }
}
