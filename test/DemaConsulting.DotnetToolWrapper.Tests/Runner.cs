using System.Diagnostics;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Program runner class
/// </summary>
internal static class Runner
{
    /// <summary>
    /// Run the specified program
    /// </summary>
    /// <param name="output">Program output</param>
    /// <param name="program">Program name</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    /// <exception cref="InvalidOperationException">On program start error</exception>
    public static int Run(out string output, string program, params string[] arguments)
    {
        // An empty working directory starts the program in the current directory
        return RunInDirectory(out output, string.Empty, program, arguments);
    }

    /// <summary>
    /// Run the specified program in the specified working directory
    /// </summary>
    /// <param name="output">Program output</param>
    /// <param name="workingDirectory">Working directory for the program (empty for the current directory)</param>
    /// <param name="program">Program name</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    /// <exception cref="InvalidOperationException">On program start error</exception>
    public static int RunInDirectory(
        out string output,
        string workingDirectory,
        string program,
        params string[] arguments)
    {
        // Construct the start information
        var startInfo = new ProcessStartInfo(program)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        };

        // Add the arguments
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        // Start the process
        using var process = Process.Start(startInfo) ??
                            throw new InvalidOperationException("Failed to start process");

        // Wait for the process to exit
        process.WaitForExit();

        // Save the combined output and return the exit code
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        output = stdout + stderr;
        return process.ExitCode;
    }
}
