# ReferenceCop Playground

This is a playground environment for testing ReferenceCop changes end-to-end.

## Structure

- `TestProject/` - Sample project that uses ReferenceCop for validation
  - `SampleApp/` - Sample application project
  - `SampleLibrary/` - Sample library project
  - `nuget.config` - Configured to use ReferenceCop package from build output
  - `Directory.Build.props` - Centralized ReferenceCop package reference with auto-version detection

## Workflow

1. **Make changes** to ReferenceCop source code
2. **Build and pack** the ReferenceCop package:
   ```powershell
   cd ..\src\ReferenceCop.Package
   dotnet pack -c Debug
   ```
   This automatically writes the package version to `package-version.txt`
3. **Restore and build** the test project:
   ```powershell
   cd playground\TestProject\SampleApp
   dotnet restore
   dotnet build
   ```
4. **Introduce test violations** or modify the `ReferenceCop.config` to test your changes

**Note**: 
- The `nuget.config` uses the package directly from the build output directory (`../../src/ReferenceCop.Package/bin/Debug`)
- The package version is automatically read from `package-version.txt` via `Directory.Build.props`
- No manual version updates needed!

## Testing Different Rule Types

The `ReferenceCop.config` includes examples of all three rule types:

1. **AssemblyName** - Blocks references to assemblies matching a pattern
2. **ProjectTag** - Blocks references between projects with specific tags
3. **ProjectPath** - Blocks references based on project folder paths

Modify the rules or project references to test violations.
