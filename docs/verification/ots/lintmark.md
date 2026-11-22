## LintMark

### Verification Approach

LintMark is verified by its self-validation. The CI quality-checks job runs
`dotnet lintmark --validate --results artifacts/lintmark-self-validation.trx` before `lint.ps1` checks the Markdown
files of the repository with the rules in `.lintmark.yaml`.

### Test Scenarios

- **LintMark_VersionDisplay** and **LintMark_HelpDisplay**: the tool starts and reports its version and usage.
- **LintMark_CleanFileNoFindings**: a conforming Markdown file passes with no findings.
- **LintMark_ViolationsReported**: rule violations are reported and the tool exits with a non-zero code.
- **LintMark_ConfigurationHonored**: the rules and settings of a configuration file are applied.
- **LintMark_FixRewritesToCanonicalForm**: `--fix` rewrites fixable violations to the canonical form used by
  `fix.ps1`.
