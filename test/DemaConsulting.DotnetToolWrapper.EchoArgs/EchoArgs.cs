using System.Globalization;

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
    /// Leading-option prefix that repeats the output, used to produce large output
    /// </summary>
    private const string RepeatPrefix = "--repeat=";

    /// <summary>
    /// Leading-option prefix that waits before printing, used to produce a long-running process
    /// </summary>
    private const string SleepPrefix = "--sleep=";

    /// <summary>
    /// Print the argument count and each argument on its own line
    /// </summary>
    /// <remarks>
    /// The first line is "count={n}"; each following line is one argument wrapped in square brackets, so empty
    /// arguments and leading or trailing spaces are visible. Leading options are consumed only while their value
    /// parses as a non-negative invariant-culture integer: "--repeat={n}" prints the output for the remaining
    /// arguments n times, which lets a test produce output larger than the command line can hold, and
    /// "--sleep={ms}" waits ms milliseconds before printing, which lets a test produce a long-running process.
    /// Any other argument, including an option whose value does not parse (such as "--repeat=abc"), ends option
    /// processing and is printed unchanged. Writes to standard output only and exits with code 0.
    /// </remarks>
    /// <param name="args">Arguments to print. Each argument may be empty or contain any characters.</param>
    public static void Main(string[] args)
    {
        // Consume the leading options whose values are valid non-negative counts
        var repeat = 1;
        var sleep = 0;
        while (args.Length > 0)
        {
            if (TryParseOption(args[0], RepeatPrefix, out var repeatValue))
            {
                repeat = repeatValue;
            }
            else if (TryParseOption(args[0], SleepPrefix, out var sleepValue))
            {
                sleep = sleepValue;
            }
            else
            {
                break;
            }

            args = args[1..];
        }

        // Wait before printing when asked to simulate a long-running program
        if (sleep > 0)
        {
            Thread.Sleep(sleep);
        }

        for (var i = 0; i < repeat; i++)
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

    /// <summary>
    /// Parse a "{prefix}{n}" option whose value must be a non-negative integer
    /// </summary>
    /// <param name="argument">Argument to parse</param>
    /// <param name="prefix">Option prefix, including the trailing equals sign</param>
    /// <param name="value">Parsed value, or 0 when the argument is not a valid option</param>
    /// <returns>True when the argument has the prefix and a valid non-negative integer value</returns>
    private static bool TryParseOption(string argument, string prefix, out int value)
    {
        value = 0;
        return argument.StartsWith(prefix, StringComparison.Ordinal) &&
               int.TryParse(argument[prefix.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out value);
    }
}
