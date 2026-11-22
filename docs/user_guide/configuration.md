# Configuration Reference

This chapter is the reference for the `DotnetToolWrapper.json` configuration file.

## File Location

The wrapper reads `DotnetToolWrapper.json` from the folder that holds
`DemaConsulting.DotnetToolWrapper.dll`. In a tool package that is each `tools/{framework}/any/`
folder, so every framework folder needs its own copy of the file. When the file is missing, the
wrapper prints `Missing configuration file` with the full path it looked for, and exits with code 1.

## File Format

The file is a JSON object. Each property name is a target name, and each value is an object with a
`program` property that holds the path of the native executable for that target:

```json
{
  "win-x64": {
    "program": "../../../win-x64/my-tool.exe"
  },
  "win-arm64": {
    "program": "../../../win-arm64/my-tool.exe"
  },
  "linux-x64": {
    "program": "../../../linux-x64/my-tool"
  },
  "linux-arm64": {
    "program": "../../../linux-arm64/my-tool"
  },
  "osx-x64": {
    "program": "../../../osx-x64/my-tool"
  },
  "osx-arm64": {
    "program": "../../../osx-arm64/my-tool"
  }
}
```

Rules:

- Include an entry only for each target the tool supports. A host whose target has no entry gets the
  error `This tool does not support the {target} target`.
- Target names and the `program` property name are case-sensitive. Write them in lower case, exactly
  as listed in the Target Names section.
- An entry without a `program` property gives the error `Bad configuration for {target} target`.
- The wrapper reads only the entry for the current target, and only its `program` property. Other
  entries and other properties are ignored.
- The file must be valid JSON. Comments and trailing commas are not allowed.

## Program Path Resolution

The wrapper turns the `program` value into the full path of the executable in two steps:

1. It expands environment variable references written as `%NAME%` (see the next section).
2. It resolves the result against the folder that holds `DotnetToolWrapper.json`:
   - A relative path, such as `../../../linux-x64/my-tool`, is relative to that folder, not to the
     user's current directory.
   - An absolute path, such as `C:/Tools/my-tool.exe` or `/opt/my-tool/my-tool`, is used as it is.

Use forward slashes (`/`) as the path separator. They work on every operating system, while a
backslash is a path separator only on Windows.

## Environment Variables

The `program` path may contain environment variable references written as `%NAME%`. The wrapper
replaces each reference with the value of the variable before it resolves the path. This syntax
works on every operating system, including Linux, FreeBSD, and macOS:

```json
{
  "win-x64": {
    "program": "%LOCALAPPDATA%/MyTool/my-tool.exe"
  },
  "linux-x64": {
    "program": "%HOME%/.local/share/mytool/my-tool"
  }
}
```

This is useful when the native executable is installed in a user-specific location rather than
shipped inside the package.

Rules:

- Only the `%NAME%` form is expanded. Shell forms such as `$HOME` and `${HOME}`, and the home
  directory shortcut `~`, are **not** expanded; they stay in the path as literal text. Write
  `%HOME%` instead of `$HOME` or `~`.
- A reference to a variable that is not defined is left unchanged, so the path keeps the literal
  `%NAME%` text and the program is not found.
- Variable names are case-insensitive on Windows, and case-sensitive on Linux, FreeBSD, and macOS.

## Target Names

A target name is `{os}-{arch}`. The operating system names are `win`, `linux`, `freebsd`, and `osx`.
The architecture names are `x86`, `x64`, `arm`, `arm64`, `wasm`, and `s390x`. A platform or
architecture the wrapper does not recognize is named `unknown`. The How DotnetToolWrapper Works
chapter describes how the names are detected.
