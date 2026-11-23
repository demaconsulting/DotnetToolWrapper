using System.Diagnostics;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Program runner class
/// </summary>
internal static class Runner
{
    /// <summary>
    /// Maximum time to wait for the program to exit
    /// </summary>
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Run the specified program
    /// </summary>
    /// <remarks>
    /// Standard output and standard error are read while the program runs, so a program that writes more than the
    /// pipe buffer holds cannot block. A program that runs longer than 60 seconds is killed with its process tree.
    /// </remarks>
    /// <param name="output">Program output: standard output followed by standard error</param>
    /// <param name="program">Program name</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    /// <exception cref="InvalidOperationException">On program start error</exception>
    /// <exception cref="TimeoutException">When the program does not exit within 60 seconds</exception>
    public static int Run(out string output, string program, params string[] arguments)
    {
        // An empty working directory starts the program in the current directory
        return RunInDirectory(out output, string.Empty, program, arguments);
    }

    /// <summary>
    /// Run the specified program in the specified working directory
    /// </summary>
    /// <remarks>
    /// Standard output and standard error are read while the program runs, so a program that writes more than the
    /// pipe buffer holds cannot block. A program that runs longer than 60 seconds is killed with its process tree.
    /// </remarks>
    /// <param name="output">Program output: standard output followed by standard error</param>
    /// <param name="workingDirectory">Working directory for the program (empty for the current directory)</param>
    /// <param name="program">Program name</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    /// <exception cref="InvalidOperationException">On program start error</exception>
    /// <exception cref="TimeoutException">When the program does not exit within 60 seconds</exception>
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

        // Read both streams while the process runs, so a full pipe buffer cannot block the process
        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        // Wait for the process to exit, killing it with its process tree if it takes too long
        if (!process.WaitForExit(Timeout))
        {
            process.Kill(entireProcessTree: true);
            throw new TimeoutException($"Process '{program}' did not exit within {Timeout.TotalSeconds} seconds");
        }

        // Save the combined output and return the exit code
        output = stdoutTask.GetAwaiter().GetResult() + stderrTask.GetAwaiter().GetResult();
        return process.ExitCode;
    }
}
