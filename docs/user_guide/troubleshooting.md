# Troubleshooting

This chapter lists the wrapper's error messages and the solutions to common problems.

## Error Messages

The wrapper writes its own error messages to standard output and exits with code 1:

| Message | Cause |
| :--- | :--- |
| `Missing configuration file {path}` | `DotnetToolWrapper.json` is not in the folder of the wrapper DLL. |
| `This tool does not support the {target} target` | The configuration file has no entry for the target of this host. |
| `Bad configuration for {target} target` | The entry for the target has no `program` property. |
| `Unable to start process {path}` | The operating system did not start the program. |

Other failures, such as a configuration file that is not valid JSON or a program that does not exist,
end with an unhandled .NET exception that describes the problem, and a non-zero exit code.

## Tool Not Found After Installation

**Problem**: after `dotnet tool install -g`, the command is not found.

**Solutions**:

- Make sure the .NET tools folder is on the path:
  - Windows: `%USERPROFILE%\.dotnet\tools`
  - Linux and macOS: `$HOME/.dotnet/tools`
- Restart the terminal, or run `dotnet tool list -g` to check that the tool is installed.

## Unsupported Platform

**Problem**: the tool prints `This tool does not support the {target} target`.

**Solutions**:

- Compare the target in the message with the entries in `DotnetToolWrapper.json`. The names must match
  exactly, in lower case, in the form `{os}-{arch}`.
- Add an entry and a native executable for the platform, if the tool should support it.
- A target that contains `unknown` means the wrapper does not recognize the operating system or the
  architecture of the host, so no entry can match it.

## Missing Configuration File

**Problem**: the tool prints `Missing configuration file` followed by a path.

**Solutions**:

- Make sure `DotnetToolWrapper.json` is in the same folder as `DemaConsulting.DotnetToolWrapper.dll`,
  in every `tools/{framework}/any/` folder of the package.
- Check that the `.nuspec` file includes the configuration file in the package.
- Check that the file can be read by the user running the tool.

## Bad Configuration

**Problem**: the tool prints `Bad configuration for {target} target`.

**Solution**: add a `program` property to the entry for that target. The property name is
case-sensitive and must be written in lower case.

## Program Not Found

**Problem**: the tool fails with a .NET exception that names the program file, for example "An error
occurred trying to start process" or "The system cannot find the file specified".

**Solutions**:

- Check the `program` path. A relative path is resolved against the folder that holds
  `DotnetToolWrapper.json`, not against the current directory.
- Check that the `.nuspec` file includes the folders of the native executables.
- Use forward slashes in the path, because a backslash is not a path separator on Linux, FreeBSD,
  or macOS.

## Environment Variable Not Expanded

**Problem**: the program path still contains `$HOME`, `~`, or `%NAME%` text, and the program is not
found.

**Solutions**:

- Write environment variable references as `%NAME%`, for example `%HOME%`. The `$HOME`, `${HOME}`,
  and `~` forms are never expanded.
- Make sure the variable is defined in the environment of the tool command. An undefined variable
  is left unchanged.
- On Linux, FreeBSD, and macOS, check the case of the variable name: `%home%` is not `%HOME%`.

## Permission Denied

**Problem**: the program cannot be started on Linux, FreeBSD, or macOS because permission is denied.

**Solutions**:

- Give the native executable execute permission before packing:

  ```bash
  chmod +x my-tool
  ```

- If the executable is already packed, extract it, fix the permission, and pack it again.

## Wrong Executable Runs

**Problem**: the tool runs the wrong executable, or fails to find the right one.

**Solutions**:

- Check the target names in `DotnetToolWrapper.json` against the Target Names section of the How
  DotnetToolWrapper Works chapter.
- Check that each `program` path points to the executable built for that target, relative to the
  folder of the configuration file.
- The `This tool does not support the {target} target` message names the target the wrapper
  detected; compare it with the entries of the configuration file.

## Examples and Support

- Find tools built with DotnetToolWrapper through the
  [dotnettoolwrapper topic](https://github.com/topics/dotnettoolwrapper) on GitHub.
- Report bugs or request features through
  [GitHub Issues](https://github.com/demaconsulting/DotnetToolWrapper/issues).
- Ask questions through
  [GitHub Discussions](https://github.com/demaconsulting/DotnetToolWrapper/discussions).
