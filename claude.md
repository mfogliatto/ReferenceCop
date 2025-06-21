# claude.md

## Project Overview

**Project Name:** ReferenceCop  
**Description:**  
ReferenceCop is a suite of tools (including a Roslyn analyzer and an MSBuild task) designed to help .NET teams enforce dependency management rules at build time. It prevents discouraged or prohibited references from being added to projects, ensuring adherence to clean architecture principles.

**Tech Stack:**  
- .NET (Roslyn analyzers & MSBuild)
- C#
- MSBuild tasks

---

## Current Status

- Ready for integration into .NET projects as a NuGet package.
- Requires configuration via `ReferenceCop.config` and MSBuild properties.
- Supports repository-wide and per-project configuration.
- Provided with sample rules and configuration in documentation.
- Inefficient detection algorithm in AssemblyNameViolationDetector is used.
- Rules cannot be configured to have exceptions. Support for NoWarn is not available and needs to be implemented.

---

## Main Objectives

1. Enforce architectural constraints by restricting certain project/package references.
2. Make configuration simple and maintainable for large .NET codebases.
3. Provide clear, actionable diagnostics during the build process.

---

## Code Style Guidelines

Please ensure all code follows StyleCop-based conventions as defined in #src\stylecop.json.

## Testing Guidelines

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
  
---

## Instructions for the AI Agent

- Write clean, idiomatic C# code.
- Follow the coding standards and style described in the sections above.
- Add XML documentation for all public methods.
- Write MSTest tests for all new code.
- If you need clarification, request it in the `[ASSISTANT NOTE]` section below.

---

## Reference

Use as reference the code that is located in #src\ReferenceCop.

## Output Format

- Return code changes as code blocks, specifying the filename and relative path.
- If making multiple file changes, group by file.
- Summarize changes below each code block.
- Ask for clarification in `[ASSISTANT NOTE]` if anything is unclear.

---

## [ASSISTANT NOTE]

(Reserved for agent to request clarification or summarize uncertainties.)

---