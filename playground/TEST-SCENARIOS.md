# E2E Test Scenarios

This document describes the automated end-to-end test scenarios run by `Run-E2ETests.ps1`.

## Scenarios

| # | Rule Type | Scenario | Expected Result |
|---|-----------|----------|-----------------|
| 1 | AssemblyName | SampleLibrary references Newtonsoft.Json (blocked by `NoNewtonsoftJson` rule) | Build fails with RC0001/RC0002 |
| 2 | ProjectPath | SampleApp references InternalLib (blocked by `AppCannotReferenceInternal` rule) | Build fails with RC0001/RC0002 |
| 3 | ProjectTag | SampleApp (tag=App) references ToolsProject (tag=Tools), blocked by `AppCannotReferenceTools` rule | Warning diagnostic RC0001/RC0002 emitted |
| 4 | Valid | ValidApp with no violations | Build succeeds cleanly |

## Running Locally

```powershell
# Pack first, then run tests
./playground/Run-E2ETests.ps1 -PackFirst

# Or if already packed:
./playground/Run-E2ETests.ps1
```

## CI Integration

The tests run automatically in both PR and official build workflows after the unit tests pass.
The workflow packs the package in Debug mode, then invokes the test runner.
