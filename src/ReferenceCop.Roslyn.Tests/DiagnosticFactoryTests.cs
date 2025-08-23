namespace ReferenceCop.Roslyn.Tests
{
    using System;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for the DiagnosticFactory class.
    /// </summary>
    [TestClass]
    public class DiagnosticFactoryTests
    {
        /// <summary>
        /// Tests that error violations create error diagnostics.
        /// </summary>
        [TestMethod]
        public void CreateFor_WhenViolationHasErrorSeverity_CreatesErrorDiagnostic()
        {
            // Arrange
            const string ruleName = "TestRule";
            const string ruleDescription = "Test rule description";
            const string referenceName = "TestReference";
            const string pattern = "TestPattern";

            var rule = new ReferenceCopConfig.AssemblyName
            {
                Name = ruleName,
                Description = ruleDescription,
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Pattern = pattern,
            };
            var violation = new Violation(rule, referenceName);

            // Act
            var diagnostic = DiagnosticFactory.CreateFor(violation);

            // Assert
            using (new AssertionScope())
            {
                diagnostic.Should().NotBeNull();
                diagnostic.Id.Should().Be(Violation.ViolationSeverityErrorCode);
                diagnostic.Severity.Should().Be(DiagnosticSeverity.Error);
                diagnostic.GetMessage().Should().Contain(referenceName);
                diagnostic.GetMessage().Should().Contain(ruleName);
                diagnostic.GetMessage().Should().Contain(ruleDescription);
            }
        }

        /// <summary>
        /// Tests that warning violations create warning diagnostics.
        /// </summary>
        [TestMethod]
        public void CreateFor_WhenViolationHasWarningSeverity_CreatesWarningDiagnostic()
        {
            // Arrange
            const string ruleName = "TestRule";
            const string ruleDescription = "Test rule description";
            const string referenceName = "TestReference";
            const string fromTag = "TestFromTag";
            const string toTag = "TestToTag";

            var rule = new ReferenceCopConfig.ProjectTag
            {
                Name = ruleName,
                Description = ruleDescription,
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Warning,
                FromProjectTag = fromTag,
                ToProjectTag = toTag,
            };
            var violation = new Violation(rule, referenceName);

            // Act
            var diagnostic = DiagnosticFactory.CreateFor(violation);

            // Assert
            using (new AssertionScope())
            {
                diagnostic.Should().NotBeNull();
                diagnostic.Id.Should().Be(Violation.ViolationSeverityWarningCode);
                diagnostic.Severity.Should().Be(DiagnosticSeverity.Warning);
                diagnostic.GetMessage().Should().Contain(referenceName);
                diagnostic.GetMessage().Should().Contain(ruleName);
                diagnostic.GetMessage().Should().Contain(ruleDescription);
            }
        }

        /// <summary>
        /// Tests that violations with unknown severities return null.
        /// </summary>
        [TestMethod]
        public void CreateFor_WhenViolationHasUnknownSeverity_ReturnsNull()
        {
            // Arrange
            const string ruleName = "TestRule";
            const string ruleDescription = "Test rule description";
            const string referenceName = "TestReference";
            const string fromPath = "TestFromPath";
            const string toPath = "TestToPath";
            const ReferenceCopConfig.Rule.ViolationSeverity invalidSeverity = (ReferenceCopConfig.Rule.ViolationSeverity)999;

            var rule = new ReferenceCopConfig.ProjectPath
            {
                Name = ruleName,
                Description = ruleDescription,
                Severity = invalidSeverity,
                FromPath = fromPath,
                ToPath = toPath,
            };
            var violation = new Violation(rule, referenceName);

            // Act
            var diagnostic = DiagnosticFactory.CreateFor(violation);

            // Assert
            diagnostic.Should().BeNull();
        }

        /// <summary>
        /// Tests that exceptions create error diagnostics.
        /// </summary>
        [TestMethod]
        public void CreateFor_WhenExceptionIsProvided_CreatesErrorDiagnostic()
        {
            // Arrange
            const string exceptionMessage = "Test exception message";
            const string generalErrorCode = "RC0000";
            var exception = new InvalidOperationException(exceptionMessage);

            // Act
            var diagnostic = DiagnosticFactory.CreateFor(exception);

            // Assert
            using (new AssertionScope())
            {
                diagnostic.Should().NotBeNull();
                diagnostic.Id.Should().Be(generalErrorCode);
                diagnostic.Severity.Should().Be(DiagnosticSeverity.Error);
                diagnostic.GetMessage().Should().Contain(exceptionMessage);
            }
        }
    }
}
