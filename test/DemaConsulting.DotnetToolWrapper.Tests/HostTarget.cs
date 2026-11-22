using System.Runtime.InteropServices;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Expected target names of the current host, derived independently of the Program class
/// </summary>
/// <remarks>
/// The tests compare the wrapper against these values, so they are computed from different .NET APIs than the
/// wrapper uses: <see cref="OperatingSystem"/> checks instead of <see cref="RuntimeInformation.IsOSPlatform"/>,
/// and the lower-case name of <see cref="RuntimeInformation.OSArchitecture"/> instead of the wrapper's mapping
/// table. The lower-case enum names equal the wrapper's architecture names for every supported architecture.
/// The tests run only on supported hosts, so a host these helpers cannot name fails the tests that use them.
/// </remarks>
internal static class HostTarget
{
    /// <summary>
    /// Get the expected operating system name of the current host
    /// </summary>
    /// <returns>"win", "linux", "freebsd", or "osx"; "unknown" on any other operating system.</returns>
    public static string ExpectedOs()
    {
        // Name the host operating system with the OperatingSystem checks
        if (OperatingSystem.IsWindows())
        {
            return "win";
        }

        if (OperatingSystem.IsLinux())
        {
            return "linux";
        }

        if (OperatingSystem.IsFreeBSD())
        {
            return "freebsd";
        }

        if (OperatingSystem.IsMacOS())
        {
            return "osx";
        }

        return "unknown";
    }

    /// <summary>
    /// Get the expected architecture name of the current host
    /// </summary>
    /// <returns>The lower-case name of the host architecture, for example "x64" or "arm64".</returns>
    public static string ExpectedArchitecture()
    {
        // The lower-case enum name matches the target name of every supported architecture
        return RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant();
    }

    /// <summary>
    /// Get the expected target string of the current host
    /// </summary>
    /// <returns>The target string "{os}-{architecture}", for example "linux-x64".</returns>
    public static string ExpectedTarget()
    {
        return $"{ExpectedOs()}-{ExpectedArchitecture()}";
    }
}
