# Changelog

All notable changes to **Robotico.Repository.MishimaDocs** will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Dual Mishima dependency: **ProjectReference** when `MishimaDocsProjectPath` exists, else **PackageReference** `MishimaDocs` (`Directory.Packages.props`).
- `publish.yml` (build, test, coverage, pack/push) and Dependabot for NuGet + GitHub Actions.
- `.editorconfig` enforcing explicit types (`IDE0008`) per Robotico Library 10/10 standard.
- CsCheck property tests for `MishimaDocsRepositoryPersistenceRouter` and `MishimaDocumentIdFormatter`.
- Unit tests for persistence routing fallbacks, document id formatting branches, CORRUPT reads, and JSON deserialization failures.
- `tests/coverlet.runsettings` and GitHub Actions CI (coverage gate ≥ 85% line rate on adapter assembly, `verify-robotico-library-bar.ps1` in umbrella layout).
- `PackageVersion` for **CsCheck** in central `Directory.Packages.props`.
- Central `MishimaDocsProjectPath` in `Directory.Build.props`.

### Changed

- MishimaDocs wiring: conditional **ProjectReference** vs **PackageReference**; fully qualified `IRepository` / `IUnitOfWork` where needed for IDE0005.
- Test projects use explicit types throughout.
