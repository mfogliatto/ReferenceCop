<#
.SYNOPSIS
    Automated end-to-end test runner for ReferenceCop playground scenarios.

.DESCRIPTION
    Builds various test configurations in the playground and validates that
    ReferenceCop correctly detects violations and allows valid references.

.PARAMETER PackFirst
    If set, builds and packs the ReferenceCop package before running tests.
#>
param(
    [switch]$PackFirst
)

$ErrorActionPreference = "Stop"
$script:TestsPassed = 0
$script:TestsFailed = 0
$script:Failures = @()

$RepoRoot = (Resolve-Path "$PSScriptRoot/..").Path
$PlaygroundRoot = "$PSScriptRoot/TestProject"

function Write-TestHeader($name) {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host " TEST: $name" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
}

function Assert-BuildFails($project, $testName, $expectedDiagnostic) {
    Write-TestHeader $testName
    $output = dotnet build "$PlaygroundRoot/$project" --no-restore 2>&1 | Out-String
    $exitCode = $LASTEXITCODE

    if ($exitCode -eq 0) {
        Write-Host "  FAIL: Build succeeded but was expected to fail" -ForegroundColor Red
        Write-Host "  Output: $output" -ForegroundColor Gray
        $script:TestsFailed++
        $script:Failures += $testName
        return
    }

    if ($expectedDiagnostic -and ($output -notmatch $expectedDiagnostic)) {
        Write-Host "  FAIL: Build failed but diagnostic '$expectedDiagnostic' not found in output" -ForegroundColor Red
        Write-Host "  Output: $output" -ForegroundColor Gray
        $script:TestsFailed++
        $script:Failures += $testName
        return
    }

    Write-Host "  PASS: Build failed as expected with diagnostic '$expectedDiagnostic'" -ForegroundColor Green
    $script:TestsPassed++
}

function Assert-BuildSucceeds($project, $testName) {
    Write-TestHeader $testName
    $output = dotnet build "$PlaygroundRoot/$project" --no-restore 2>&1 | Out-String
    $exitCode = $LASTEXITCODE

    if ($exitCode -ne 0) {
        Write-Host "  FAIL: Build failed but was expected to succeed" -ForegroundColor Red
        Write-Host "  Output: $output" -ForegroundColor Gray
        $script:TestsFailed++
        $script:Failures += $testName
        return
    }

    Write-Host "  PASS: Build succeeded as expected" -ForegroundColor Green
    $script:TestsPassed++
}

# --- Setup ---

if ($PackFirst) {
    Write-Host "`nPacking ReferenceCop..." -ForegroundColor Yellow
    Push-Location "$RepoRoot/src/ReferenceCop.Package"
    dotnet pack -c Debug --nologo -v quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Host "FATAL: Failed to pack ReferenceCop" -ForegroundColor Red
        exit 1
    }
    Pop-Location
}

# Restore all playground projects
Write-Host "`nRestoring playground projects..." -ForegroundColor Yellow
dotnet restore "$PlaygroundRoot/SampleApp/SampleApp.csproj" --nologo -v quiet
dotnet restore "$PlaygroundRoot/ValidApp/ValidApp.csproj" --nologo -v quiet

# --- Test Scenarios ---

# Scenario 1: AssemblyName rule - SampleLibrary references Newtonsoft.Json which is blocked
Assert-BuildFails "SampleLibrary" `
    "AssemblyName rule: Newtonsoft.Json blocked" `
    "RC000[12]"

# Scenario 2: ProjectPath rule - SampleApp referencing InternalLib should be blocked
# We need to temporarily add a project reference for this test
$sampleAppCsproj = "$PlaygroundRoot/SampleApp/SampleApp.csproj"
$originalContent = Get-Content $sampleAppCsproj -Raw

# Add InternalLib reference
$modifiedContent = $originalContent -replace '(</ItemGroup>\s*</Project>)', @"
</ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\InternalLib\InternalLib.csproj" />
  </ItemGroup>

</Project>
"@
# Fix: replace last </Project> properly
$modifiedContent = $originalContent -replace '(<ItemGroup>\s*<ProjectReference Include="\.\.\\SampleLibrary)', @"
<ItemGroup>
    <ProjectReference Include="..\InternalLib\InternalLib.csproj" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SampleLibrary"
"@

# Simpler approach: just write the test project with InternalLib reference
$testCsproj = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <ProjectTag>App</ProjectTag>
  </PropertyGroup>
  <ItemGroup>
    <CompilerVisibleProperty Include="LaunchDebugger" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\InternalLib\InternalLib.csproj" />
  </ItemGroup>
</Project>
"@

# Save original, write modified, test, restore
$originalSampleApp = Get-Content $sampleAppCsproj -Raw
Set-Content $sampleAppCsproj -Value $testCsproj -NoNewline
dotnet restore "$sampleAppCsproj" --nologo -v quiet 2>$null

Assert-BuildFails "SampleApp" `
    "ProjectPath rule: App cannot reference Internal" `
    "RC000[12]"

# Restore original
Set-Content $sampleAppCsproj -Value $originalSampleApp -NoNewline
dotnet restore "$sampleAppCsproj" --nologo -v quiet 2>$null

# Scenario 3: ProjectTag rule - App referencing Tools project should warn
$toolsTestCsproj = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <ProjectTag>App</ProjectTag>
  </PropertyGroup>
  <ItemGroup>
    <CompilerVisibleProperty Include="LaunchDebugger" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\ToolsProject\ToolsProject.csproj" />
  </ItemGroup>
</Project>
"@

Set-Content $sampleAppCsproj -Value $toolsTestCsproj -NoNewline
dotnet restore "$sampleAppCsproj" --nologo -v quiet 2>$null

# ProjectTag rule has Severity=Warning, so build may succeed but with warnings
# We check for the diagnostic in output regardless of exit code
Write-TestHeader "ProjectTag rule: App cannot reference Tools (warning)"
$output = dotnet build "$PlaygroundRoot/SampleApp" --no-restore 2>&1 | Out-String
if ($output -match "RC000[12]") {
    Write-Host "  PASS: ProjectTag violation diagnostic detected" -ForegroundColor Green
    $script:TestsPassed++
} else {
    Write-Host "  FAIL: Expected ProjectTag warning diagnostic not found" -ForegroundColor Red
    Write-Host "  Output: $output" -ForegroundColor Gray
    $script:TestsFailed++
    $script:Failures += "ProjectTag rule: App cannot reference Tools (warning)"
}

# Restore original
Set-Content $sampleAppCsproj -Value $originalSampleApp -NoNewline
dotnet restore "$sampleAppCsproj" --nologo -v quiet 2>$null

# Scenario 4: Valid project builds successfully (no violations)
Assert-BuildSucceeds "ValidApp" `
    "Valid project: No violations, build succeeds"

# --- Summary ---
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " RESULTS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Passed: $script:TestsPassed" -ForegroundColor Green
Write-Host "  Failed: $script:TestsFailed" -ForegroundColor $(if ($script:TestsFailed -gt 0) { "Red" } else { "Green" })

if ($script:TestsFailed -gt 0) {
    Write-Host "`n  Failed tests:" -ForegroundColor Red
    foreach ($f in $script:Failures) {
        Write-Host "    - $f" -ForegroundColor Red
    }
    exit 1
}

Write-Host "`n  All e2e tests passed!" -ForegroundColor Green
exit 0
