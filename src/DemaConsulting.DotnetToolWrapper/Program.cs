using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace DemaConsulting.DotnetToolWrapper;

/// <summary>
/// Dotnet Tool Wrapper Program
/// </summary>
public class Program
{
    /// <summary>
    /// Get the operating system
    /// </summary>
    /// <returns>Operating system name</returns>
    public static string GetOs()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "win";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "linux";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) return "freebsd";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "osx";
        return "unknown";
    }

    /// <summary>
    /// Get the architecture
    /// </summary>
    /// <returns>Architecture name</returns>
    public static string GetArchitecture()
    {
        return RuntimeInformation.OSArchitecture switch
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
        return $"{GetOs()}-{GetArchitecture()}";
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
            location = workingDirectory;

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
            startInfo.ArgumentList.Add(arg);

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
