# Security Policy

## Supported Versions

We release patches for security vulnerabilities for the following versions:

| Version | Supported          |
| ------- | ------------------ |
| Latest  | :white_check_mark: |
| < Latest| :x:                |

We recommend always using the latest version of DotnetToolWrapper to ensure you have the latest security updates.

## Reporting a Vulnerability

We take the security of DotnetToolWrapper seriously. If you believe you have found a security vulnerability, please
report it to us as described below.

### How to Report

**Please do NOT report security vulnerabilities through public GitHub issues.**

Instead, please report them via GitHub's private vulnerability reporting feature:

1. Navigate to the [Security tab][github-security] of the repository
2. Click "Report a vulnerability"
3. Fill out the vulnerability report form with as much detail as possible

Alternatively, you can email the maintainers directly through the contact information available in the repository.

### What to Include

Please include the following information in your report:

- **Type of vulnerability** (e.g., code injection, privilege escalation, etc.)
- **Full paths of source file(s)** related to the vulnerability
- **Location of the affected source code** (tag/branch/commit or direct URL)
- **Step-by-step instructions to reproduce** the issue
- **Proof-of-concept or exploit code** (if possible)
- **Impact of the issue**, including how an attacker might exploit it
- **Any potential mitigations** you've identified

### What to Expect

- **Acknowledgment**: We will acknowledge receipt of your vulnerability report within 48 hours
- **Updates**: We will send you regular updates about our progress at least every 7 days
- **Timeline**: We aim to resolve critical vulnerabilities within 30 days
- **Credit**: We will credit you in the security advisory unless you prefer to remain anonymous

## Security Considerations

### Configuration File Trust

DotnetToolWrapper reads `DotnetToolWrapper.json` to determine which native executable to launch. This configuration
file must be trusted:

- The wrapper does not validate paths in the configuration file
- Malicious configuration files could execute arbitrary programs
- **Only use DotnetToolWrapper packages from trusted sources**

### Argument Handling

- Command-line arguments are passed directly to the native executable without modification
- The wrapper does not perform input validation or sanitization
- Native executables must handle their own input validation

### Native Executable Trust

- DotnetToolWrapper executes native binaries specified in the configuration
- **Ensure native executables are from trusted sources and have been scanned for malware**
- The wrapper has no mechanism to verify the integrity of native executables

### Environment Variable Expansion

- Configuration paths support environment variable expansion
- Malicious environment variables could redirect execution to unintended programs
- Use caution when running in environments with untrusted environment variables

### Process Execution Model

- The wrapper uses `ProcessStartInfo` with `UseShellExecute = false`
- This avoids shell interpretation of arguments, reducing injection risks
- However, native executables may still be vulnerable to their own security issues

## Best Practices for Package Creators

If you're creating a .NET tool package using DotnetToolWrapper:

1. **Scan Native Executables**: Use antivirus and malware scanning on all native executables
2. **Verify Sources**: Only include native executables from trusted, verified sources
3. **Sign Packages**: Sign your NuGet packages to ensure authenticity
4. **Document Security**: Document any security considerations specific to your tool
5. **Update Dependencies**: Keep DotnetToolWrapper updated to the latest version
6. **Minimal Privileges**: Ensure your native executables require minimal system privileges
7. **Input Validation**: Implement proper input validation in your native executables

## Best Practices for Package Users

If you're using a .NET tool package built with DotnetToolWrapper:

1. **Trust Sources**: Only install tools from trusted publishers
2. **Review Packages**: Review package contents before installation
3. **Keep Updated**: Regularly update installed tools
4. **Verify Signatures**: Check NuGet package signatures
5. **Audit Usage**: Monitor what tools are installed and their behavior
6. **Sandboxing**: Consider running tools in isolated environments for sensitive operations

## Known Limitations

### No Signature Verification

DotnetToolWrapper does not verify digital signatures of native executables. Package creators and users must ensure
executable authenticity through other means.

### No Sandboxing

The wrapper does not provide sandboxing or isolation of native executables. Native executables run with the same
permissions as the user who invoked the tool.

### Configuration File Security

The wrapper trusts the configuration file completely. There is no mechanism to prevent a compromised configuration
file from executing arbitrary programs.

## Security Update Process

When a security vulnerability is confirmed:

1. **Assessment**: We assess the severity and impact
2. **Fix Development**: We develop and test a fix
3. **Advisory Creation**: We create a GitHub Security Advisory
4. **Release**: We release a patched version
5. **Notification**: We notify users through:
   - GitHub Security Advisories
   - Release notes
   - Repository README updates

## Disclosure Policy

We follow a coordinated disclosure process:

- We request 90 days to address vulnerabilities before public disclosure
- We will work with you to determine an appropriate disclosure timeline
- We will credit you in the security advisory unless you prefer anonymity
- We ask that you do not publicly disclose the vulnerability until we've released a fix

## Security-Related Configuration

### Recommended .gitignore Entries

Ensure your .gitignore includes:

```gitignore
# Sensitive files
*.key
*.pem
*.pfx
*.p12
secrets.json
appsettings.*.json

# Build artifacts that might contain secrets
bin/
obj/
```

## Contact

For security-related questions that are not vulnerability reports:

- Open a GitHub issue with the "security" label
- Contact maintainers through the repository's contact information

## Attribution

This security policy is based on security best practices from:

- [Microsoft Security Development Lifecycle][microsoft-sdl]
- [OWASP Secure Coding Practices][owasp-secure-coding]
- [GitHub Security Advisories][github-security-advisories]

## Updates to This Policy

We may update this security policy from time to time. We will notify users of material changes by:

- Updating the file in the repository
- Mentioning changes in release notes
- Creating a GitHub discussion for significant changes

[github-security]: https://github.com/demaconsulting/DotnetToolWrapper/security
[microsoft-sdl]: https://www.microsoft.com/en-us/securityengineering/sdl/
[owasp-secure-coding]: https://owasp.org/www-project-secure-coding-practices-quick-reference-guide/
[github-security-advisories]: https://docs.github.com/en/code-security/security-advisories
