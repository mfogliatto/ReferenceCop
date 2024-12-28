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
            var result = detector.GetViolationsFrom(new List<AssemblyIdentity> { new AssemblyIdentity("System.Xml.Linq") });

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
                new AssemblyIdentity(detectableValue),
                new AssemblyIdentity("System.Xml.Serialization"),
                new AssemblyIdentity("System.Xml.Linq"),
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
                new AssemblyIdentity(detectableValue1),
                new AssemblyIdentity(detectableValue2),
                new AssemblyIdentity("System.Text"),
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
                new AssemblyIdentity("System.Xml.Serialization"),
                new AssemblyIdentity("System.Xml.Linq"),
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
            var diagnostics = detector.GetViolationsFrom(Array.Empty<AssemblyIdentity>());

            // Assert
            diagnostics.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNullReference_ThrowsInvalidOperationException()
        {
            // Arrange.
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule("somePattern")
                .Build();
            var detector = new AssemblyNameViolationDetector(Substitute.For<IEqualityComparer<string>>(), config);
            var references = new AssemblyIdentity[]
            {
                null,
            };

            // Act
            Action act = () => detector.GetViolationsFrom(references).First();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
