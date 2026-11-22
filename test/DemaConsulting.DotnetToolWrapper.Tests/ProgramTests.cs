namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Unit tests for the Program class methods
/// </summary>
public class ProgramTests
{
    /// <summary>
    /// Test GetOs method returns valid OS identifier
    /// </summary>
    [Fact]
    public void Program_GetOs_CurrentHost_ReturnsKnownOsIdentifier()
    {
        // Arrange: (none - GetOs inspects the current host)

        // Act: get the OS identifier of the current host
        var os = Program.GetOs();

        // Assert: identifier is one of the values the wrapper can produce
        Assert.NotNull(os);
        Assert.True(
            os is "win" or "linux" or "freebsd" or "osx" or "unknown",
            $"OS should be one of the known values, got: {os}");
    }

    /// <summary>
    /// Test GetArchitecture method returns valid architecture identifier
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_CurrentHost_ReturnsKnownArchitectureIdentifier()
    {
        // Arrange: (none - GetArchitecture inspects the current host)

        // Act: get the architecture identifier of the current host
        var arch = Program.GetArchitecture();

        // Assert: identifier is one of the values the wrapper can produce
        Assert.NotNull(arch);
        Assert.True(
            arch is "x86" or "x64" or "arm" or "arm64" or "wasm" or "s390x" or "unknown",
            $"Architecture should be one of the known values, got: {arch}");
    }

    /// <summary>
    /// Test GetTarget method returns valid target string
    /// </summary>
    [Fact]
    public void Program_GetTarget_CurrentHost_ReturnsOsAndArchitectureTarget()
    {
        // Arrange: (none - GetTarget inspects the current host)

        // Act: get the target string of the current host
        var target = Program.GetTarget();

        // Assert: target is two hyphen-separated parts
        Assert.NotNull(target);
        Assert.Contains("-", target);

        var parts = target.Split('-');
        Assert.Equal(2, parts.Length);
    }
}
