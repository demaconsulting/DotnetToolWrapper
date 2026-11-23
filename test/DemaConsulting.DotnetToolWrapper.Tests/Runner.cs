using System.Diagnostics;

namespace DemaConsulting.DotnetToolWrapper.Tests;

/// <summary>
/// Program runner class
/// </summary>
internal static class Runner
{
    /// <summary>
    /// Default maximum time to wait for the program to exit
    /// </summary>
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Maximum time to wait for a killed program to exit and for its output streams to close
    /// </summary>
    private static readonly TimeSpan KillWaitTimeout = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Key of the <see cref="Exception.Data"/> entry holding the process ID of a program that timed out
    /// </summary>
    public const string ProcessIdDataKey = "ProcessId";

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
        return RunCore(out output, string.Empty, DefaultTimeout, program, arguments);
    }

    /// <summary>
    /// Run the specified program with the specified timeout
    /// </summary>
    /// <remarks>
    /// Standard output and standard error are read while the program runs, so a program that writes more than the
    /// pipe buffer holds cannot block. A program that runs longer than the timeout is killed with its process tree,
    /// and the call waits for it to exit before throwing.
    /// </remarks>
    /// <param name="output">Program output: standard output followed by standard error</param>
    /// <param name="timeout">Maximum time to wait for the program to exit</param>
    /// <param name="program">Program name</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    /// <exception cref="InvalidOperationException">On program start error</exception>
    /// <exception cref="TimeoutException">
    /// When the program does not exit within the timeout; <see cref="Exception.Data"/> holds the process ID under
    /// <see cref="ProcessIdDataKey"/>
    /// </exception>
    public static int RunWithTimeout(out string output, TimeSpan timeout, string program, params string[] arguments)
    {
        // An empty working directory starts the program in the current directory
        return RunCore(out output, string.Empty, timeout, program, arguments);
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
        return RunCore(out output, workingDirectory, DefaultTimeout, program, arguments);
    }

    /// <summary>
    /// Run the specified program in the specified working directory with the specified timeout
    /// </summary>
    /// <param name="output">Program output: standard output followed by standard error</param>
    /// <param name="workingDirectory">Working directory for the program (empty for the current directory)</param>
    /// <param name="timeout">Maximum time to wait for the program to exit</param>
    /// <param name="program">Program name</param>
    /// <param name="arguments">Program arguments</param>
    /// <returns>Program exit code</returns>
    /// <exception cref="InvalidOperationException">On program start error</exception>
    /// <exception cref="TimeoutException">When the program does not exit within the timeout</exception>
    private static int RunCore(
        out string output,
        string workingDirectory,
        TimeSpan timeout,
        string program,
        string[] arguments)
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
        if (!process.WaitForExit(timeout))
        {
            KillAndWait(process, stdoutTask, stderrTask);
            var exception = new TimeoutException(
                $"Process '{program}' (PID {process.Id}) did not exit within {timeout.TotalSeconds} seconds");
            exception.Data[ProcessIdDataKey] = process.Id;
            throw exception;
        }

        // Save the combined output and return the exit code
        output = stdoutTask.GetAwaiter().GetResult() + stderrTask.GetAwaiter().GetResult();
        return process.ExitCode;
    }

    /// <summary>
    /// Kill a process with its process tree, then wait for it to exit and for its output streams to close
    /// </summary>
    /// <param name="process">Process to kill</param>
    /// <param name="stdoutTask">Task reading standard output</param>
    /// <param name="stderrTask">Task reading standard error</param>
    private static void KillAndWait(Process process, Task<string> stdoutTask, Task<string> stderrTask)
    {
        // Kill the process tree; the process may have exited on its own after the wait timed out
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
            // The process has already exited, which is the goal of the kill
        }

        // Wait for the process to actually exit so it does not outlive the call
        process.WaitForExit(KillWaitTimeout);

        // Observe the stream reads so that they finish and any failure does not go unobserved
        try
        {
            Task.WaitAll([stdoutTask, stderrTask], KillWaitTimeout);
        }
        catch (AggregateException)
        {
            // The output of a killed process is discarded, so read failures are ignored
        }
    }
}
