namespace DemaConsulting.DotnetToolWrapper.EchoArgs;

/// <summary>
/// Test helper program that prints its arguments
/// </summary>
/// <remarks>
/// The system tests configure the wrapper to run this program so they can check that every argument reaches the
/// wrapped program unchanged. Shell commands cannot show this on every platform because the shell re-parses its
/// command line, so this helper receives the arguments from the .NET runtime and prints them without any
/// interpretation.
/// </remarks>
public static class EchoArgs
{
    /// <summary>
    /// Print the argument count and each argument on its own line
    /// </summary>
    /// <remarks>
    /// The first line is "count={n}"; each following line is one argument wrapped in square brackets, so empty
    /// arguments and leading or trailing spaces are visible. Writes to standard output only and exits with code 0.
    /// </remarks>
    /// <param name="args">Arguments to print. Each argument may be empty or contain any characters.</param>
    public static void Main(string[] args)
    {
        // Print the count first so that a missing or extra argument is detected
        Console.WriteLine($"count={args.Length}");

        // Print each argument in brackets so that empty arguments and surrounding spaces are visible
        foreach (var arg in args)
        {
            Console.WriteLine($"[{arg}]");
        }
    }
}
