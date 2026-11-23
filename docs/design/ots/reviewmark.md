## ReviewMark

ReviewMark (`DemaConsulting.ReviewMark`) is a .NET local tool that tracks the formal review of files in review-sets.

### Purpose

Continuous Compliance requires evidence that every file that defines, implements, or verifies the wrapper has been
reviewed, and that each review is still valid for the current content. ReviewMark is chosen because it groups the
files into named review-sets from a checked-in configuration, fingerprints their content, and generates the review
plan and review report from that configuration and the review evidence, without a separate review database.

### Features Used

- **Configuration lint** - `--lint` checks `.reviewmark.yaml` for structural problems.
- **Review plan** - `--plan` writes the Markdown list of review-sets and the files each one covers.
- **Review report** - `--report` writes the Markdown status of each review-set against the review evidence.
- **Enforcement** - `--enforce` exits with a non-zero code when a generated document has issues; with `--plan`,
  when a file needing review is not covered by any review-set.
- **Self-validation** - `--validate --results {file}` writes a TRX file of the built-in tests.

### Integration Pattern

ReviewMark is restored from the local tool manifest and started as `dotnet reviewmark`. `lint.ps1` runs
`dotnet reviewmark --lint`, which fails the quality checks on a configuration problem, and generates a discarded
review plan with `--enforce`, which fails them when a file needing review is not covered by a review-set. The CI
documents job runs the self-validation, then `dotnet reviewmark --plan docs/code_review_plan/generated/plan.md
--enforce` and `dotnet reviewmark --report docs/code_review_report/generated/report.md`, and Pandoc and WeasyPrint
build the Review Plan and Review Report from the generated Markdown. The configuration reads the review evidence from
the `index.json` of the orphan `reviews` branch (`type: url`), which also holds the review PDFs. No reviews are
recorded yet, so the report shows every review-set without current evidence and is generated without `--enforce`;
enforcing the review evidence follows once reviews are recorded. Each run is a single process with no state.
