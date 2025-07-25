# claude.md

## Project Overview

**Project Name:** ReferenceCop  
**Description:**  
ReferenceCop is a suite of tools (including a Roslyn analyzer and an MSBuild task) designed to help .NET teams enforce dependency management rules at build time. It prevents discouraged or prohibited references from being added to projects, ensuring adherence to clean architecture principles.

**Tech Stack:**  
- .NET (Roslyn analyzers & MSBuild)
- C#
- MSBuild tasks

## Main Objectives

1. Enforce architectural constraints by restricting certain project/package references.
2. Make configuration simple and maintainable for large .NET codebases.
3. Provide clear, actionable diagnostics during the build process.

## Architecture Patterns

### Component Structure
- **Core Library**: Contains rule configuration models, violation detectors, and providers
- **MSBuild Integration**: Provides build-time validation using the Task pattern
- **Roslyn Integration**: Provides IDE-time validation using the DiagnosticAnalyzer pattern
- **Package**: Aggregates components into a single distributable NuGet package

### Key Abstractions
- `IViolationDetector<T>`: Core interface for all rule validators
- `ReferenceCopConfig`: Configuration model for rule definitions
- `Violation`: Represents a rule violation with severity and messaging

### Rule Types
- `AssemblyName`: Validates referenced assembly names against patterns
- `ProjectTag`: Validates project references based on project tag metadata
- `ProjectPath`: Validates project references based on folder structure

## Development Workflow

### Build and Test
```bash
dotnet build src/ReferenceCop.sln
dotnet test src/ReferenceCop.sln
```

### Package Creation
```bash
# For local development builds
dotnet pack src/ReferenceCop.Package/ReferenceCop.Package.csproj -c Debug

# For release builds
dotnet pack src/ReferenceCop.Package/ReferenceCop.Package.csproj -c Release
```

### Debugging
- Set `LaunchDebugger` property in MSBuild to trigger debugging:
  - For MSBuild task: `LaunchDebugger=MSBuild`
  - For Roslyn analyzer: `LaunchDebugger=Roslyn`

## Integration Testing
- The project uses itself for validation (dogfooding)
- Special handling in `.Package.csproj` to avoid circular references during development

## Contribution Guidelines

### Code Style Guidelines

Please ensure all code follows StyleCop-based conventions as defined in the project's configuration file located at `src\stylecop.json`. This file defines the following important style guidelines:

1. **Documentation Rules**: Internal elements, private fields, and interfaces don't require documentation comments.
2. **Ordering Rules**: 
   - Using directives should be placed inside namespaces
   - System using directives should be listed first
3. **Layout Rules**: All files must end with a newline

When adding or modifying code, adhere to these style conventions for consistency across the project.

### Testing Guidelines

**Unit Test Conventions:**

1. **Test Naming**:  
   Use `MethodName_WhenScenario_DoesSomething` for test method names.  
   *Example*:  
   `CalculateTotal_WhenDiscountIsApplied_ReducesFinalPrice`

2. **Mocking**:  
   - Use **NSubstitute** for simple substitutions.  
   - **Inline mocks**: Define NSubstitute substitutions inline (where used) if no setup is needed.  
   - **Use builders if available**: Use existing builders if present in the project; otherwise, NSubstitute.

3. **Assertions**:  
   Use **FluentAssertions** with expressive syntax:  
   *Example*:  
   `result.Should().Be(expectedValue).Because("the discount should apply");`

4. **Test Structure**:  
   Explicitly separate test phases with comments:  
```

// Arrange.
var service = Substitute.For<IService>();

// Act.
var result = sut.Calculate();

// Assert.
result.Should().NotBeNull();
```

5. **Framework**:  
Use **MSTest** attributes:  
- Annotate test classes with `[TestClass]`  
- Test methods with `[TestMethod]`  
- Setup/teardown with `[TestInitialize]`/`[TestCleanup]`  
- Data-driven tests with `[DataTestMethod]` + `[DataRow]`

6. **Readability**:  
- Keep tests focused on a single scenario. Mock only dependencies critical to the tested behavior.
- When arranging and asserting in tests, **avoid duplicating strings or values**.  
  Instead, assign these values to variables (or constants, when appropriate) in the Arrange section, 
  and reuse those variables in the Assert section. This ensures maintainability and clarity.

### Pull Request Guidelines

When creating pull requests, always use the repository's pull request template located at `.github/PULL_REQUEST_TEMPLATE.md`. This template includes the following sections:

- Description of changes
- Type of change (Enhancement, Bug fix, Chore)
- Related issue link
- Testing information
- Screenshots (if applicable)
- Additional context

Ensure all required fields in the template are filled out appropriately when creating pull requests.

### Common Tasks

#### Adding a New Rule Type
1. Create rule class inheriting from `ReferenceCopConfig.Rule`
2. Add to `XmlArrayItem` attributes in `ReferenceCopConfig`
3. Create detector implementing `IViolationDetector<T>`
4. Update task/analyzer to use the new detector

#### Modifying Rule Processing
- Core detection logic in `Detectors/` namespace
- MSBuild integration in `ReferenceCopTask.cs`
- Roslyn integration in `ReferenceCopAnalyzer.cs`

## Instructions for the AI Agent

- Write clean, idiomatic C# code.
- Follow the coding standards and style described in the sections above.
- Add XML documentation for all public methods.
- Write MSTest tests for all new code.
- If you need clarification, request it in the `[ASSISTANT NOTE]` section below.

## Reference

Use as reference the code that is located in #src\ReferenceCop.

## Output Format

- Return code changes as code blocks, specifying the filename and relative path.
- If making multiple file changes, group by file.
- Summarize changes below each code block.
- Ask for clarification in `[ASSISTANT NOTE]` if anything is unclear.