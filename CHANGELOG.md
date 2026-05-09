# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [Unreleased]

### Added

- Ralph onboarding — updated `.gitignore` and added MCP configuration ([#47](https://github.com/mfogliatto/ReferenceCop/pull/47))
- Experimental detectors for development and Copilot documentation ([#46](https://github.com/mfogliatto/ReferenceCop/pull/46))

## [0.0.8-alpha] - 2025-12-24

### Added

- Playground environment for end-to-end testing ([#44](https://github.com/mfogliatto/ReferenceCop/pull/44))

### Changed

- Optimized `AssemblyNameViolationDetector` to O(n) time complexity ([#45](https://github.com/mfogliatto/ReferenceCop/pull/45))

## [0.0.7-alpha] - 2025-12-20

### Added

- `nuget.config` to ensure nuget.org source is always included ([#41](https://github.com/mfogliatto/ReferenceCop/pull/41))

### Fixed

- AssemblyName rules loading only picking up first rule of set ([#42](https://github.com/mfogliatto/ReferenceCop/pull/42))

### Changed

- Updated dogfood version ([#40](https://github.com/mfogliatto/ReferenceCop/pull/40))

## [0.0.6-alpha] - 2025-09-14

### Changed

- Improved Roslyn analyzer configuration for better performance and compliance ([#39](https://github.com/mfogliatto/ReferenceCop/pull/39))

## [0.0.5-alpha] - 2025-08-23

### Added

- Roslyn Analyzer NoWarn support and other improvements ([#38](https://github.com/mfogliatto/ReferenceCop/pull/38))
- NoWarn support to prevent warnings for specific references — MSBuild detectors only ([#37](https://github.com/mfogliatto/ReferenceCop/pull/37))
- Copilot instructions with improved contribution guidelines ([#35](https://github.com/mfogliatto/ReferenceCop/pull/35))
- Project memory file for AI agent context ([#34](https://github.com/mfogliatto/ReferenceCop/pull/34))
- DiagnosticFactory tests ([#20](https://github.com/mfogliatto/ReferenceCop/pull/20))

### Changed

- Refactored `ReferenceCopTask` with dependency injection and added test coverage ([#22](https://github.com/mfogliatto/ReferenceCop/pull/22))
- Autoincrement version, fix local feed import, and version updates ([#33](https://github.com/mfogliatto/ReferenceCop/pull/33))

### Fixed

- StyleCop Roslyn analyzer warning ([#32](https://github.com/mfogliatto/ReferenceCop/pull/32))
- MSBuild task output when no errors are logged ([#21](https://github.com/mfogliatto/ReferenceCop/pull/21))

## [0.0.4-alpha] - 2025-05-03

### Added

- Dogfooding: consume ReferenceCop published package ([#19](https://github.com/mfogliatto/ReferenceCop/pull/19))

### Changed

- Moved PackageId from Package csproj to official build workflow ([#18](https://github.com/mfogliatto/ReferenceCop/pull/18))
- Updated branch ruleset ([#17](https://github.com/mfogliatto/ReferenceCop/pull/17))
- Updated README.md ([#16](https://github.com/mfogliatto/ReferenceCop/pull/16))

## [0.0.3-alpha] - 2025-02-15

### Fixed

- XSD ReferenceCopConfig schema ([#15](https://github.com/mfogliatto/ReferenceCop/pull/15))
- Package publish step condition ([#12](https://github.com/mfogliatto/ReferenceCop/pull/12))
- Build step tag condition ([#13](https://github.com/mfogliatto/ReferenceCop/pull/13))

### Added

- SourceLink and deterministic builds ([#14](https://github.com/mfogliatto/ReferenceCop/pull/14))

## [0.0.2-alpha] - 2025-02-06

### Added

- Initial NuGet package publishing workflow ([#2](https://github.com/mfogliatto/ReferenceCop/pull/2))
- Contributing resources ([#3](https://github.com/mfogliatto/ReferenceCop/pull/3))
- .NET Analyzer support and other changes ([#4](https://github.com/mfogliatto/ReferenceCop/pull/4))

### Fixed

- MSBuild error: path format not supported ([#5](https://github.com/mfogliatto/ReferenceCop/pull/5))
- PKGVERSION issues ([#6](https://github.com/mfogliatto/ReferenceCop/pull/6), [#7](https://github.com/mfogliatto/ReferenceCop/pull/7), [#8](https://github.com/mfogliatto/ReferenceCop/pull/8), [#9](https://github.com/mfogliatto/ReferenceCop/pull/9), [#10](https://github.com/mfogliatto/ReferenceCop/pull/10), [#11](https://github.com/mfogliatto/ReferenceCop/pull/11))
- Build workflow ([#1](https://github.com/mfogliatto/ReferenceCop/pull/1))

[Unreleased]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.8-alpha...HEAD
[0.0.8-alpha]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.7-alpha...v0.0.8-alpha
[0.0.7-alpha]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.6-alpha...v0.0.7-alpha
[0.0.6-alpha]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.5-alpha...v0.0.6-alpha
[0.0.5-alpha]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.4-alpha...v0.0.5-alpha
[0.0.4-alpha]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.3-alpha...v0.0.4-alpha
[0.0.3-alpha]: https://github.com/mfogliatto/ReferenceCop/compare/v0.0.2-alpha...v0.0.3-alpha
[0.0.2-alpha]: https://github.com/mfogliatto/ReferenceCop/releases/tag/v0.0.2-alpha
