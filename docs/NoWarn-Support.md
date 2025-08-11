# NoWarn Support in ReferenceCop

ReferenceCop now supports the `NoWarn` attribute for project references, allowing you to suppress specific warnings for particular references.

## How to Use

### In MSBuild Projects (.csproj)

To suppress ReferenceCop warnings for a specific project reference, add the `NoWarn` attribute to the `ProjectReference` element in your project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- Suppress ReferenceCop warnings RC0002 for this reference -->
    <ProjectReference Include="..\SomeProject\SomeProject.csproj" NoWarn="RC0002" />
    
    <!-- Suppress multiple ReferenceCop warnings for this reference -->
    <ProjectReference Include="..\AnotherProject\AnotherProject.csproj" NoWarn="RC0001,RC0002" />
    
    <!-- No NoWarn attribute means warnings will be shown -->
    <ProjectReference Include="..\ThirdProject\ThirdProject.csproj" />
  </ItemGroup>
</Project>
```

Note that when specifying multiple warning codes, they should be separated by commas (,), not semicolons (;).

### ReferenceCop Warning Codes

- `RC0001`: Illegal reference (Error)
- `RC0002`: Discouraged reference (Warning)

## Implementation Notes

The NoWarn support works in both:

1. **MSBuild Task**: Direct compilation through MSBuild will respect the NoWarn attributes.
2. **Roslyn Analyzer**: When editing in Visual Studio or other IDEs, the Roslyn analyzer will also respect the NoWarn attributes.

The implementation uses compiler-visible properties to pass the NoWarn information from the MSBuild task to the Roslyn analyzer, ensuring consistent behavior between build-time and design-time.
