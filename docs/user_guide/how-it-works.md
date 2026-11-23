# How DotnetToolWrapper Works

This chapter explains the parts of a wrapped tool package and what the wrapper does each time the tool
runs.

## Package Roles

A wrapped tool package holds four kinds of files:

- **The wrapper** (`DemaConsulting.DotnetToolWrapper.dll` with its `.deps.json` and
  `.runtimeconfig.json` files) is the .NET entry point of the tool. The .NET SDK starts it when the
  user runs the tool command.
- **`DotnetToolSettings.xml`** tells the .NET SDK the name of the tool command and that its entry
  point is the wrapper.
- **`DotnetToolWrapper.json`** maps each supported platform to the native executable to run.
- **The native executables**, one for each supported platform, do the real work.

The .NET SDK treats the package like any other .NET Tool: it installs it, puts the command on the
path, and starts the wrapper on the installed .NET runtime. The native executables are never started
by the SDK itself; only the wrapper starts them.

## Run-Time Sequence

The following diagram shows the decisions the wrapper makes each time the tool command runs. The
left column is the normal path, and the red boxes on the right are the configuration errors, which
all end with exit code 1.

![Flowchart of the wrapper at run time: it detects the target, checks the configuration file, the target entry, and its program property, and exits with code 1 when a check fails; otherwise it resolves the path, starts the program, and exits with its exit code](wrapper-flow.mmd){scale=2 width=80%}

In detail, the wrapper:

1. Builds the target name, `{os}-{arch}`, from the operating system and processor architecture it
   runs on (see the Target Names section below).
2. Reads `DotnetToolWrapper.json` from the folder that holds `DemaConsulting.DotnetToolWrapper.dll`.
3. Looks up the entry for the target name, and the `program` property of that entry.
4. Expands any `%NAME%` environment variable references in the program path, then resolves a
   relative path against the folder of the configuration file.
5. Starts the program with the user's arguments, in the user's current directory.
6. Waits for the program to exit, then exits with the program's exit code.

## Target Names

The target name combines an operating system name and an architecture name with a hyphen, for example
`win-x64`, `linux-arm64`, or `osx-arm64`. The names follow the style of the .NET runtime identifiers.

The wrapper checks the operating systems in the order shown, and the first match wins:

| Operating System | Name |
| :--- | :--- |
| Windows | `win` |
| Linux | `linux` |
| FreeBSD | `freebsd` |
| macOS | `osx` |
| Any other | `unknown` |

The architecture is the processor architecture of the operating system, as reported by .NET:

| Architecture | Name |
| :--- | :--- |
| 32-bit Intel or AMD | `x86` |
| 64-bit Intel or AMD | `x64` |
| 32-bit ARM | `arm` |
| 64-bit ARM, including Apple Silicon | `arm64` |
| WebAssembly | `wasm` |
| IBM System z | `s390x` |
| Any other, for example LoongArch64, Armv6, or Ppc64le | `unknown` |

## Process Behavior

The wrapper starts the native program directly, without a shell:

- **Arguments**: every argument given to the tool command is passed to the program unchanged and in
  order. No shell expansion, quoting, or globbing is applied by the wrapper.
- **Working directory**: the program runs in the user's current directory, so relative file paths in
  its arguments work as the user expects.
- **Console**: standard input, output, and error are not redirected. The program reads and writes the
  same console as the user's shell, so interactive programs and colored output work.
- **Environment**: the program inherits the environment variables of the tool command.
- **Exit code**: the wrapper waits for the program to exit and then exits with the same exit code, so
  scripts and build systems see the program's result.
