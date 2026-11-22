## Program

### Verification Approach

Program is verified by the unit tests in `ProgramTests.cs`. The target-naming logic is split into public methods
that read the current host (`GetOs()`, `GetArchitecture()`, `GetTarget()`) and internal overloads that take the
platform check (`Func<OSPlatform, bool>`) and the processor architecture as parameters. The tests inject a platform
check that reports a single simulated operating system and pass architecture values directly, so every operating
system and architecture mapping is verified on any build machine, including FreeBSD, x86, arm, wasm, and s390x,
for which there is no CI runner. Nothing else is mocked: the methods are pure functions of their inputs.

Three tests call the public methods on the real host to verify that the production wiring returns one of the
documented names and that `GetTarget()` joins the results of `GetOs()` and `GetArchitecture()`.

`Main` reads the configuration file beside the wrapper DLL, starts a process, and ends the wrapper process with the
program's exit code, so it is not called from the unit tests; the system tests in the DotnetToolWrapper
verification design run it as a real process and cover the configuration handling, path resolution, process
start, and exit code.

### Test Environment

N/A - standard test environment; the tests need no files, environment variables, or external processes.

### Acceptance Criteria

- All unit tests pass with zero failures on every CI runner and every target framework.
- Each supported operating system and architecture maps to its documented name, and any other value maps to
  `unknown`.
- The target name is the operating system name and the architecture name joined by a hyphen.

### Test Scenarios

**Operating system names**: `Program_GetOs_WindowsPlatform_ReturnsWin`, `Program_GetOs_LinuxPlatform_ReturnsLinux`,
`Program_GetOs_FreeBsdPlatform_ReturnsFreeBsd`, and `Program_GetOs_OsxPlatform_ReturnsOsx` simulate a host that
reports only that platform and expect `win`, `linux`, `freebsd`, and `osx`.

**Unknown operating system**: `Program_GetOs_UnknownPlatform_ReturnsUnknown` simulates a host that reports none of
the supported platforms and expects `unknown`.

**Platform priority**: `Program_GetOs_MultiplePlatformsMatch_ReturnsFirstInPriorityOrder` simulates a host that
reports both macOS and Linux and expects `linux`, the first match in the order Windows, Linux, FreeBSD, macOS.

**Missing platform check**: `Program_GetOs_NullPlatformCheck_ThrowsArgumentNullException` passes no platform check
and expects `ArgumentNullException`.

**Architecture names**: `Program_GetArchitecture_X86_ReturnsX86`, `Program_GetArchitecture_X64_ReturnsX64`,
`Program_GetArchitecture_Arm_ReturnsArm`, `Program_GetArchitecture_Arm64_ReturnsArm64`,
`Program_GetArchitecture_Wasm_ReturnsWasm`, and `Program_GetArchitecture_S390x_ReturnsS390x` pass each supported
architecture and expect `x86`, `x64`, `arm`, `arm64`, `wasm`, and `s390x`.

**Unknown architecture**: `Program_GetArchitecture_UnsupportedArchitecture_ReturnsUnknown` passes `LoongArch64`,
`Armv6`, `Ppc64le`, and the undefined value 999, and expects `unknown` for each.

**Target names**: `Program_GetTarget_WindowsX64_ReturnsWinX64`, `Program_GetTarget_LinuxArm64_ReturnsLinuxArm64`,
and `Program_GetTarget_UnknownPlatformAndArchitecture_ReturnsUnknownUnknown` expect `win-x64`, `linux-arm64`, and
`unknown-unknown`.

**Current host**: `Program_GetOs_CurrentHost_ReturnsKnownOsIdentifier`,
`Program_GetArchitecture_CurrentHost_ReturnsKnownArchitectureIdentifier`, and
`Program_GetTarget_CurrentHost_ReturnsOsAndArchitectureTarget` call the public methods on the build machine and
expect a documented name and a two-part target equal to `{GetOs()}-{GetArchitecture()}`.
