namespace ReferenceCop.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class AssemblyNameViolationDetectorTests
    {
        [TestMethod]
        public void GetViolationsFrom_WhenNoRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig();
            var detector = new AssemblyNameViolationDetector(Substitute.For<IEqualityComparer<string>>(), config);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<AssemblyIdentity>>
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity("System.Xml.Linq")),
            });

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMatchingRule_ReturnsViolation()
        {
            // Arrange.
            const string detectableValue = "System.Xml";
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule(detectableValue)
                .Build();
            var comparer = Substitute.For<IEqualityComparer<string>>();
            comparer.Equals(detectableValue, detectableValue).Returns(true);
            var detector = new AssemblyNameViolationDetector(comparer, config);
            var references = new[]
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(detectableValue)),
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity("System.Xml.Serialization")),
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity("System.Xml.Linq")),
            };

            // Act
            var diagnostics = detector.GetViolationsFrom(references);

            // Assert
            diagnostics.Should().HaveCount(1);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMatchingRuleUsesPatternMatch_ReturnsViolations()
        {
            // Arrange.
            const string partialMatch = "System.Xml";
            var detectablePattern = $"{partialMatch}.*";
            var detectableValue1 = $"{partialMatch}.Serialization";
            var detectableValue2 = $"{partialMatch}.Linq";
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule(detectablePattern)
                .Build();
            var detector = new AssemblyNameViolationDetector(new PatternMatchComparer(), config);
            var references = new[]
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(detectableValue1)),
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(detectableValue2)),
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity("System.Text")),
            };

            // Act
            var diagnostics = detector.GetViolationsFrom(references);

            // Assert
            diagnostics.Should().HaveCount(2);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoMatchingRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfigBuilder().Build();
            var detector = new AssemblyNameViolationDetector(Substitute.For<IEqualityComparer<string>>(), config);
            var references = new[]
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity("System.Xml.Serialization")),
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity("System.Xml.Linq")),
            };

            // Act
            var diagnostics = detector.GetViolationsFrom(references);

            // Assert
            diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoReferences_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule("somePattern")
                .Build();
            var detector = new AssemblyNameViolationDetector(Substitute.For<IEqualityComparer<string>>(), config);

            // Act
            var diagnostics = detector.GetViolationsFrom(Array.Empty<ReferenceEvaluationContext<AssemblyIdentity>>());

            // Assert
            diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenWarningSuppressed_SkipsViolation()
        {
            // Arrange.
            const string detectableValue = "System.Xml";
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule(detectableValue)
                .Build();
            var comparer = Substitute.For<IEqualityComparer<string>>();
            comparer.Equals(detectableValue, detectableValue).Returns(true);
            var detector = new AssemblyNameViolationDetector(comparer, config);
            var references = new[]
            {
                // Create with warning suppressed
                new ReferenceEvaluationContext<AssemblyIdentity>(new AssemblyIdentity(detectableValue), isWarningSuppressed: true),
            };

            // Act
            var diagnostics = detector.GetViolationsFrom(references);

            // Assert
            diagnostics.Should().BeEmpty("because the violation was suppressed");
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMultipleReferencesWithSomeSuppressed_ReturnsSomeViolations()
        {
            // Arrange.
            const string partialMatch = "System.Xml";
            var detectablePattern = $"{partialMatch}.*";
            var detectableValue1 = $"{partialMatch}.Serialization";
            var detectableValue2 = $"{partialMatch}.Linq";
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule(detectablePattern)
                .Build();
            var detector = new AssemblyNameViolationDetector(new PatternMatchComparer(), config);
            var references = new[]
            {
                // Regular reference with no suppression
                new ReferenceEvaluationContext<AssemblyIdentity>(new AssemblyIdentity(detectableValue1), isWarningSuppressed: false),

                // Suppressed reference
                new ReferenceEvaluationContext<AssemblyIdentity>(new AssemblyIdentity(detectableValue2), isWarningSuppressed: true),

                // Non-matching reference
                new ReferenceEvaluationContext<AssemblyIdentity>(new AssemblyIdentity("System.Text"), isWarningSuppressed: false),
            };

            // Act
            var diagnostics = detector.GetViolationsFrom(references);

            // Assert
            diagnostics.Should().ContainSingle()
                .Which.ReferenceName.Should().Be(detectableValue1);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNullReference_ThrowsInvalidOperationException()
        {
            // Arrange.
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule("somePattern")
                .Build();
            var detector = new AssemblyNameViolationDetector(Substitute.For<IEqualityComparer<string>>(), config);
            var references = new ReferenceEvaluationContext<AssemblyIdentity>[]
            {
                ReferenceEvaluationContextFactory.Create<AssemblyIdentity>(null),
            };

            // Act
            Action act = () => detector.GetViolationsFrom(references).First();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
