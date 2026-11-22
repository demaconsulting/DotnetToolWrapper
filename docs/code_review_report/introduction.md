# Introduction

This document contains the review report for DotnetToolWrapper.

## Purpose

The review report shows, for each review-set defined in `.reviewmark.yaml`, whether a formal review has been
recorded for the current content of its files. It is evidence that the reviews remain valid for the files of this
release.

## Scope

The review report covers:

- the status of each review-set (current, stale, or missing review evidence);
- the fingerprint of the files of each review-set, compared with the fingerprint recorded by its review.

## Generation Source

ReviewMark generates the report on every build by fingerprinting the files of each review-set and comparing them
with the review evidence named by the `evidence-source` of `.reviewmark.yaml`.

## Audience

This document is intended for:

- maintainers and reviewers who track which reviews must be conducted or repeated;
- quality assurance staff and auditors who check that the reviews are current for this release.
