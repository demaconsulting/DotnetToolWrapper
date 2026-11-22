using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DemaConsulting.DotnetToolWrapper;

/// <summary>
/// Dotnet Tool Wrapper Program
/// </summary>
public static class Program
{
    /// <summary>
    /// Get the operating system
    /// </summary>
    /// <returns>Operating system name</returns>
    public static string GetOs()
    {
        return GetOs(RuntimeInformation.IsOSPlatform);
    }

    /// <summary>
    /// Get the operating system name using the supplied platform check
    /// </summary>
    /// <remarks>
    /// This overload exists so the operating-system mapping can be unit-tested for every supported
    /// platform on any host; <see cref="GetOs()"/> calls it with <see cref="RuntimeInformation.IsOSPlatform"/>.
    /// Platforms are checked in the order Windows, Linux, FreeBSD, then macOS, and the first match wins.
    /// The method has no side effects other than calling <paramref name="isOsPlatform"/>, and is
    /// thread-safe if <paramref name="isOsPlatform"/> is.
    /// </remarks>
    /// <param name="isOsPlatform">
    /// Returns true when the host runs the given <see cref="OSPlatform"/>. Must not be null.
    /// </param>
    /// <returns>
    /// The operating system name used in configuration targets: "win", "linux", "freebsd", or "osx";
    /// "unknown" when no platform matches.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="isOsPlatform"/> is null.</exception>
    internal static string GetOs(Func<OSPlatform, bool> isOsPlatform)
    {
        // Reject a missing platform check - without it no platform can be identified
        ArgumentNullException.ThrowIfNull(isOsPlatform);

        // Return the name of the first matching platform, in priority order
        if (isOsPlatform(OSPlatform.Windows))
        {
            return "win";
        }

        if (isOsPlatform(OSPlatform.Linux))
        {
            return "linux";
        }

        if (isOsPlatform(OSPlatform.FreeBSD))
        {
            return "freebsd";
        }

        if (isOsPlatform(OSPlatform.OSX))
        {
            return "osx";
        }

        return "unknown";
    }

    /// <summary>
    /// Get the architecture
    /// </summary>
    /// <returns>Architecture name</returns>
    public static string GetArchitecture()
    {
        return GetArchitecture(RuntimeInformation.OSArchitecture);
    }

    /// <summary>
    /// Get the architecture name for the supplied processor architecture
    /// </summary>
    /// <remarks>
    /// This overload exists so the architecture mapping can be unit-tested for every supported
    /// architecture on any host; <see cref="GetArchitecture()"/> calls it with
    /// <see cref="RuntimeInformation.OSArchitecture"/>. The method is pure and thread-safe.
    /// </remarks>
    /// <param name="architecture">Processor architecture to map. Any value is accepted.</param>
    /// <returns>
    /// The architecture name used in configuration targets: "x86", "x64", "arm", "arm64", "wasm", or "s390x";
    /// "unknown" for any other architecture (for example LoongArch64, Armv6, Ppc64le, or an undefined value).
    /// </returns>
    internal static string GetArchitecture(Architecture architecture)
    {
        return architecture switch
        {
            Architecture.X86 => "x86",
            Architecture.X64 => "x64",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            Architecture.Wasm => "wasm",
            Architecture.S390x => "s390x",
            _ => "unknown"
        };
    }

    /// <summary>
    /// Get the target string
    /// </summary>
    /// <returns>Target string</returns>
    public static string GetTarget()
    {
        return GetTarget(RuntimeInformation.IsOSPlatform, RuntimeInformation.OSArchitecture);
    }

    /// <summary>
    /// Get the target string for the supplied platform check and processor architecture
    /// </summary>
    /// <remarks>
    /// This overload exists so the composition of the target string can be unit-tested for any platform on
    /// any host; <see cref="GetTarget()"/> calls it with <see cref="RuntimeInformation.IsOSPlatform"/> and
    /// <see cref="RuntimeInformation.OSArchitecture"/>. The target string selects the entry in the
    /// DotnetToolWrapper.json configuration file. The method has no side effects other than calling
    /// <paramref name="isOsPlatform"/>, and is thread-safe if <paramref name="isOsPlatform"/> is.
    /// </remarks>
    /// <param name="isOsPlatform">
    /// Returns true when the host runs the given <see cref="OSPlatform"/>. Must not be null.
    /// </param>
    /// <param name="architecture">Processor architecture of the host. Any value is accepted.</param>
    /// <returns>
    /// The target string "{os}-{architecture}" built from <see cref="GetOs(Func{OSPlatform, bool})"/> and
    /// <see cref="GetArchitecture(Architecture)"/>, for example "linux-x64" or "unknown-unknown".
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="isOsPlatform"/> is null.</exception>
    internal static string GetTarget(Func<OSPlatform, bool> isOsPlatform, Architecture architecture)
    {
        return $"{GetOs(isOsPlatform)}-{GetArchitecture(architecture)}";
    }

    /// <summary>
    /// Application entry point
    /// </summary>
    /// <param name="args">Program arguments</param>
    public static void Main(string[] args)
    {
        // Get the target
        var target = GetTarget();

        // Get the working directory
        var workingDirectory = Directory.GetCurrentDirectory();

        // Get the tool location
        var location = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        if (string.IsNullOrEmpty(location))
        {
            location = workingDirectory;
        }

        // Get the configuration file path
        var configPath = Path.Combine(location, "DotnetToolWrapper.json");
        if (!File.Exists(configPath))
        {
            Console.WriteLine($"Missing configuration file {configPath}");
            Environment.Exit(1);
        }

        // Read the configuration
        var config = JsonDocument.Parse(File.ReadAllText(configPath));

        // Get the target configuration
        if (!config.RootElement.TryGetProperty(target, out var targetConfig))
        {
            Console.WriteLine($"This tool does not support the {target} target");
            Environment.Exit(1);
        }

        // Get the application
        if (!targetConfig.TryGetProperty("program", out var program))
        {
            Console.WriteLine($"Bad configuration for {target} target");
            Environment.Exit(1);
        }

        // Get the program name
        var programName = program.ToString();
        programName = Environment.ExpandEnvironmentVariables(programName);
        programName = Path.GetFullPath(programName, location);

        // Construct the process start information
        var startInfo = new ProcessStartInfo(programName)
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false
        };
        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        // Run the process
        var process = Process.Start(startInfo);
        if (process == null)
        {
            Console.WriteLine($"Unable to start process {programName}");
            Environment.Exit(1);
        }

        // Wait for the process to exit
        process.WaitForExit();

        // Exit with the process exit code
        Environment.Exit(process.ExitCode);
    }
}
