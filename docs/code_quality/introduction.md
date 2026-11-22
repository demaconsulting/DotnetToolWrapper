# Introduction

This document describes the code quality controls applied to DotnetToolWrapper.

## Purpose

This report shows how the code of DotnetToolWrapper is kept free of compiler warnings, analyzer findings, formatting
deviations, and security problems, and where the results of each control are found. A build of this version exists
only because every control that runs in the build passed.

## Scope

This report covers:

- the compiler and the static code analyzers that run in every build;
- the formatting and text checks of the quality-checks job;
- the CodeQL code scanning of the repository.

SonarCloud analysis is not used by this project.

## Analysis Source

The controls are configured in the project file of the wrapper, in `.editorconfig`, in `lint.ps1`, and in the
CodeQL default setup of the GitHub repository. The build fails when a compiler or analyzer control reports a
problem, so this document describes the controls rather than listing findings. CodeQL results are kept by GitHub
code scanning.

## Audience

This document is intended for:

- maintainers and contributors who need to know which checks a change must pass;
- quality assurance staff and auditors evaluating the code quality evidence of a release.
