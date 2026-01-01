using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Unit tests for the Program class methods
/// </summary>
[TestClass]
public class ProgramTests
{
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
        Assert.Contains("-", target, "Target should contain a hyphen");
        
        var parts = target.Split('-');
        Assert.HasCount(2, parts, "Target should have exactly two parts");
    }
}
