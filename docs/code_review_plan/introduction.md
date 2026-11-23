# Introduction

This document contains the review plan for DotnetToolWrapper.

## Purpose

The review plan lists every file of DotnetToolWrapper that requires formal review and the named review-sets that
cover it. It is evidence that each such file is covered by at least one review-set.

## Scope

The review plan covers the files selected by the `needs-review` patterns of `.reviewmark.yaml`:

- the README and the User Guide;
- the C# source and test files;
- the requirements files;
- the design and verification documentation;
- the SysML2 architecture model.

## Generation Source

ReviewMark generates the plan on every build from the `.reviewmark.yaml` configuration, and the build fails when a
file requiring review is not covered by any review-set. Each review-set groups the files that answer one review
question, from the user-facing purpose down to the Program unit and each OTS item.

## Audience

This document is intended for:

- maintainers and reviewers who plan and conduct the formal reviews;
- quality assurance staff and auditors who check that every file requiring review is covered.
