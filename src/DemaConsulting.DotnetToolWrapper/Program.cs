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
    /// <remarks>
    /// Names the operating system of the host for the target string that selects the configuration entry. It
    /// calls <see cref="GetOs(Func{OSPlatform, bool})"/> with <see cref="RuntimeInformation.IsOSPlatform"/>, so
    /// the platforms are checked in the order Windows, Linux, FreeBSD, then macOS. The method only reads runtime
    /// information, has no side effects, and is thread-safe.
    /// </remarks>
    /// <returns>Operating system name: "win", "linux", "freebsd", "osx", or "unknown" on any other platform</returns>
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
    /// <remarks>
    /// Names the processor architecture of the host operating system for the target string that selects the
    /// configuration entry. It calls <see cref="GetArchitecture(Architecture)"/> with
    /// <see cref="RuntimeInformation.OSArchitecture"/> (the operating system architecture, not the architecture of
    /// the running process). The method only reads runtime information, has no side effects, and is thread-safe.
    /// </remarks>
    /// <returns>
    /// Architecture name: "x86", "x64", "arm", "arm64", "wasm", "s390x", or "unknown" for any other architecture
    /// </returns>
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
    /// <remarks>
    /// Builds the name of the DotnetToolWrapper.json entry that <see cref="Main"/> looks up for the host. It
    /// calls <see cref="GetTarget(Func{OSPlatform, bool}, Architecture)"/> with
    /// <see cref="RuntimeInformation.IsOSPlatform"/> and <see cref="RuntimeInformation.OSArchitecture"/>. The method
    /// only reads runtime information, has no side effects, and is thread-safe.
    /// </remarks>
    /// <returns>Target string "{os}-{architecture}", for example "win-x64" or "linux-arm64"</returns>
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
    /// <remarks>
    /// <para>
    /// Runs the native program configured for the host target in the DotnetToolWrapper.json file beside the
    /// wrapper assembly. The configured program path has environment variable references written as %NAME%
    /// expanded and is resolved against the wrapper folder; the program starts in the caller's working
    /// directory with each argument passed unchanged, and shares the wrapper's console.
    /// </para>
    /// <para>
    /// The method never returns: it ends the process with <see cref="Environment.Exit"/>, using the exit code of
    /// the program once it has finished. It writes a message to standard output and exits with code 1 when the
    /// configuration file is missing, when the configuration has no entry for the host target, when that entry
    /// has no <c>program</c> property, or (defensively) when <see cref="Process.Start(ProcessStartInfo)"/> returns
    /// null.
    /// </para>
    /// <para>
    /// All other errors are not caught; the .NET runtime prints the exception and ends the process with a non-zero
    /// exit code. These include a <see cref="JsonException"/> when the configuration file is not valid JSON, an
    /// <see cref="InvalidOperationException"/> when the root or the target value is not a JSON object, an
    /// <see cref="IOException"/> or <see cref="UnauthorizedAccessException"/> when the configuration file cannot
    /// be read, and a <see cref="System.ComponentModel.Win32Exception"/> when the configured program cannot be
    /// started (for example because it is missing or not executable).
    /// </para>
    /// </remarks>
    /// <param name="args">Program arguments, passed unchanged to the configured program</param>
    /// <exception cref="JsonException">The configuration file is not valid JSON.</exception>
    /// <exception cref="InvalidOperationException">The root or the target value is not a JSON object.</exception>
    /// <exception cref="IOException">The configuration file cannot be read.</exception>
    /// <exception cref="UnauthorizedAccessException">The configuration file cannot be accessed.</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">The configured program cannot be started.</exception>
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
