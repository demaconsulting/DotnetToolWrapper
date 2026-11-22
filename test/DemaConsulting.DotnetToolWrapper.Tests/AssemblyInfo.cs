namespace DemaConsulting.DotnetToolWrapper.Tests;

// The system tests write DotnetToolWrapper.json next to the wrapper DLL, so they must not run in parallel.
/// <summary>
///     Defines the Sequential test collection.
/// </summary>
/// <remarks>
///     Tests in this collection are disabled from running in parallel to prevent race conditions on the
///     shared DotnetToolWrapper.json configuration file written next to the wrapper DLL.
/// </remarks>
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class SequentialCollection;
