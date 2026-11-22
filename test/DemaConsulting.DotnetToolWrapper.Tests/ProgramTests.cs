using System.Runtime.InteropServices;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Unit tests for the Program class methods
/// </summary>
public class ProgramTests
{
    /// <summary>
    /// Test GetOs names the operating system of the current host
    /// </summary>
    [Fact]
    public void Program_GetOs_CurrentHost_ReturnsHostOsName()
    {
        // Arrange: name the host operating system independently with the OperatingSystem checks
        var expected = HostTarget.ExpectedOs();

        // Act: get the OS name of the current host
        var os = Program.GetOs();

        // Assert: the supported host is named, and with the independently derived name
        Assert.NotEqual("unknown", os);
        Assert.Equal(expected, os);
    }

    /// <summary>
    /// Test GetArchitecture names the architecture of the current host
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_CurrentHost_ReturnsHostArchitectureName()
    {
        // Arrange: name the host architecture independently from the lower-case OSArchitecture name
        var expected = HostTarget.ExpectedArchitecture();

        // Act: get the architecture name of the current host
        var arch = Program.GetArchitecture();

        // Assert: the supported host is named, and with the independently derived name
        Assert.NotEqual("unknown", arch);
        Assert.Equal(expected, arch);
    }

    /// <summary>
    /// Test GetTarget names the target of the current host
    /// </summary>
    [Fact]
    public void Program_GetTarget_CurrentHost_ReturnsHostTarget()
    {
        // Arrange: build the host target independently of the Program class
        var expected = HostTarget.ExpectedTarget();

        // Act: get the target string of the current host
        var target = Program.GetTarget();

        // Assert: target is the independently derived "{os}-{architecture}" of the host
        Assert.Equal(expected, target);
    }

    /// <summary>
    /// Create a platform check that reports only the specified platform as the host
    /// </summary>
    /// <param name="platform">Platform to simulate</param>
    /// <returns>Platform check returning true only for <paramref name="platform"/></returns>
    private static Func<OSPlatform, bool> OnlyPlatform(OSPlatform platform)
    {
        return candidate => candidate == platform;
    }

    /// <summary>
    /// Test GetOs maps a Windows host to "win"
    /// </summary>
    [Fact]
    public void Program_GetOs_WindowsPlatform_ReturnsWin()
    {
        // Arrange: simulate a Windows host
        var isOsPlatform = OnlyPlatform(OSPlatform.Windows);

        // Act: map the simulated host to an OS name
        var os = Program.GetOs(isOsPlatform);

        // Assert: Windows maps to "win"
        Assert.Equal("win", os);
    }

    /// <summary>
    /// Test GetOs maps a Linux host to "linux"
    /// </summary>
    [Fact]
    public void Program_GetOs_LinuxPlatform_ReturnsLinux()
    {
        // Arrange: simulate a Linux host
        var isOsPlatform = OnlyPlatform(OSPlatform.Linux);

        // Act: map the simulated host to an OS name
        var os = Program.GetOs(isOsPlatform);

        // Assert: Linux maps to "linux"
        Assert.Equal("linux", os);
    }

    /// <summary>
    /// Test GetOs maps a FreeBSD host to "freebsd"
    /// </summary>
    [Fact]
    public void Program_GetOs_FreeBsdPlatform_ReturnsFreeBsd()
    {
        // Arrange: simulate a FreeBSD host
        var isOsPlatform = OnlyPlatform(OSPlatform.FreeBSD);

        // Act: map the simulated host to an OS name
        var os = Program.GetOs(isOsPlatform);

        // Assert: FreeBSD maps to "freebsd"
        Assert.Equal("freebsd", os);
    }

    /// <summary>
    /// Test GetOs maps a macOS host to "osx"
    /// </summary>
    [Fact]
    public void Program_GetOs_OsxPlatform_ReturnsOsx()
    {
        // Arrange: simulate a macOS host
        var isOsPlatform = OnlyPlatform(OSPlatform.OSX);

        // Act: map the simulated host to an OS name
        var os = Program.GetOs(isOsPlatform);

        // Assert: macOS maps to "osx"
        Assert.Equal("osx", os);
    }

    /// <summary>
    /// Test GetOs maps a host matching none of the supported platforms to "unknown"
    /// </summary>
    [Fact]
    public void Program_GetOs_UnknownPlatform_ReturnsUnknown()
    {
        // Arrange: simulate a host that matches no supported platform
        Func<OSPlatform, bool> isOsPlatform = _ => false;

        // Act: map the simulated host to an OS name
        var os = Program.GetOs(isOsPlatform);

        // Assert: an unrecognized platform maps to "unknown"
        Assert.Equal("unknown", os);
    }

    /// <summary>
    /// Test GetOs returns the highest-priority platform when several platforms match
    /// </summary>
    [Fact]
    public void Program_GetOs_MultiplePlatformsMatch_ReturnsFirstInPriorityOrder()
    {
        // Arrange: simulate a host that reports both macOS and Linux (Linux is checked first)
        Func<OSPlatform, bool> isOsPlatform = platform => platform == OSPlatform.OSX || platform == OSPlatform.Linux;

        // Act: map the simulated host to an OS name
        var os = Program.GetOs(isOsPlatform);

        // Assert: the first platform in priority order (Windows, Linux, FreeBSD, macOS) wins
        Assert.Equal("linux", os);
    }

    /// <summary>
    /// Test GetOs rejects a missing platform check
    /// </summary>
    [Fact]
    public void Program_GetOs_NullPlatformCheck_ThrowsArgumentNullException()
    {
        // Arrange: no platform check
        Func<OSPlatform, bool> isOsPlatform = null!;

        // Act / Assert: mapping without a platform check throws
        Assert.Throws<ArgumentNullException>(() => Program.GetOs(isOsPlatform));
    }

    /// <summary>
    /// Test GetArchitecture maps x86 to "x86"
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_X86_ReturnsX86()
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map x86 to an architecture name
        var arch = Program.GetArchitecture(Architecture.X86);

        // Assert: x86 maps to "x86"
        Assert.Equal("x86", arch);
    }

    /// <summary>
    /// Test GetArchitecture maps x64 to "x64"
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_X64_ReturnsX64()
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map x64 to an architecture name
        var arch = Program.GetArchitecture(Architecture.X64);

        // Assert: x64 maps to "x64"
        Assert.Equal("x64", arch);
    }

    /// <summary>
    /// Test GetArchitecture maps 32-bit ARM to "arm"
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_Arm_ReturnsArm()
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map 32-bit ARM to an architecture name
        var arch = Program.GetArchitecture(Architecture.Arm);

        // Assert: 32-bit ARM maps to "arm"
        Assert.Equal("arm", arch);
    }

    /// <summary>
    /// Test GetArchitecture maps 64-bit ARM to "arm64"
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_Arm64_ReturnsArm64()
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map 64-bit ARM to an architecture name
        var arch = Program.GetArchitecture(Architecture.Arm64);

        // Assert: 64-bit ARM maps to "arm64"
        Assert.Equal("arm64", arch);
    }

    /// <summary>
    /// Test GetArchitecture maps WebAssembly to "wasm"
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_Wasm_ReturnsWasm()
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map WebAssembly to an architecture name
        var arch = Program.GetArchitecture(Architecture.Wasm);

        // Assert: WebAssembly maps to "wasm"
        Assert.Equal("wasm", arch);
    }

    /// <summary>
    /// Test GetArchitecture maps IBM s390x to "s390x"
    /// </summary>
    [Fact]
    public void Program_GetArchitecture_S390x_ReturnsS390x()
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map s390x to an architecture name
        var arch = Program.GetArchitecture(Architecture.S390x);

        // Assert: s390x maps to "s390x"
        Assert.Equal("s390x", arch);
    }

    /// <summary>
    /// Test GetArchitecture maps architectures without a target name to "unknown"
    /// </summary>
    /// <param name="architecture">Architecture without a target name</param>
    [Theory]
    [InlineData(Architecture.LoongArch64)]
    [InlineData(Architecture.Armv6)]
    [InlineData(Architecture.Ppc64le)]
    [InlineData((Architecture)999)]
    public void Program_GetArchitecture_UnsupportedArchitecture_ReturnsUnknown(Architecture architecture)
    {
        // Arrange: (none - the architecture is passed directly)

        // Act: map the unsupported architecture to an architecture name
        var arch = Program.GetArchitecture(architecture);

        // Assert: an unsupported architecture maps to "unknown"
        Assert.Equal("unknown", arch);
    }

    /// <summary>
    /// Test GetTarget composes the OS and architecture names of a Windows x64 host
    /// </summary>
    [Fact]
    public void Program_GetTarget_WindowsX64_ReturnsWinX64()
    {
        // Arrange: simulate a Windows host
        var isOsPlatform = OnlyPlatform(OSPlatform.Windows);

        // Act: build the target string for a Windows x64 host
        var target = Program.GetTarget(isOsPlatform, Architecture.X64);

        // Assert: target is "{os}-{architecture}"
        Assert.Equal("win-x64", target);
    }

    /// <summary>
    /// Test GetTarget composes the OS and architecture names of a Linux arm64 host
    /// </summary>
    [Fact]
    public void Program_GetTarget_LinuxArm64_ReturnsLinuxArm64()
    {
        // Arrange: simulate a Linux host
        var isOsPlatform = OnlyPlatform(OSPlatform.Linux);

        // Act: build the target string for a Linux arm64 host
        var target = Program.GetTarget(isOsPlatform, Architecture.Arm64);

        // Assert: target is "{os}-{architecture}"
        Assert.Equal("linux-arm64", target);
    }

    /// <summary>
    /// Test GetTarget composes "unknown" names for an unrecognized host
    /// </summary>
    [Fact]
    public void Program_GetTarget_UnknownPlatformAndArchitecture_ReturnsUnknownUnknown()
    {
        // Arrange: simulate a host that matches no supported platform
        Func<OSPlatform, bool> isOsPlatform = _ => false;

        // Act: build the target string for an unrecognized host and architecture
        var target = Program.GetTarget(isOsPlatform, Architecture.LoongArch64);

        // Assert: both parts of the target are "unknown"
        Assert.Equal("unknown-unknown", target);
    }
}
