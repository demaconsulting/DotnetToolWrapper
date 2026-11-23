## TextLint

### Verification Approach

TextLint is verified by its self-validation. The CI quality-checks job runs
`dotnet textlint --validate --results artifacts/textlint-self-validation.trx` before `lint.ps1` spell-checks the
repository with the files and settings in `.textlint.yaml`.

### Test Scenarios

- **TextLintTool_VersionDisplay** and **TextLintTool_HelpDisplay**: the tool starts and reports its version and
  usage.
- **TextLintTool_CheckCleanFile**: a correctly spelled file passes.
- **TextLintTool_CheckMisspelling**: a misspelled word is reported and the tool exits with a non-zero code.
- **TextLintTool_CheckSettingsFile** and **TextLintTool_CheckSettingsError**: a settings file is applied, and an
  invalid one is reported.
- **TextLintTool_CheckGlobsReplaceFiles** and **TextLintTool_CheckOverrides**: file globs and per-file overrides
  are honored.
- **TextLintTool_CheckTechnicalTerms**: words in the technical-terms list are accepted.
- **TextLintTool_CheckLanguageOption**: the configured language is used.
- **TextLintTool_CheckInlineDirective**: inline directives in a file are honored.
- **TextLintTool_CheckBinaryFileSkipped**: binary files are skipped.
- **TextLintTool_CheckCspellTokenization**: words are tokenized compatibly with CSpell.
