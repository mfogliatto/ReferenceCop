namespace ReferenceCop.Tests
{
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using System.Collections.Generic;

    [TestClass]
    public class AssemblyNameViolationDetectorTests
    {
        [TestMethod]
        public void GetViolationsFrom_WhenViolationIsFoundWithExactMatch_ReturnsDiagnosticEntry()
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
        public void GetViolationsFrom_WhenViolationIsFoundWithPatternMatch_ReturnsDiagnosticEntry()
        {
            // Arrange.
            const string partialMatch = "System.Xml";
            var detectablePattern = $"{partialMatch}.*";
            var detectableValue1 = $"{partialMatch}.Serialization";
            var detectableValue2 = $"{partialMatch}.Linq";
            var config = new ReferenceCopConfigBuilder()
                .WithAssemblyNameRule(detectablePattern)
                .Build();
            var comparer = Substitute.For<IEqualityComparer<string>>();
            comparer.Equals(detectablePattern, detectableValue1).Returns(true);
            comparer.Equals(detectablePattern, detectableValue2).Returns(true);
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
        public void GetViolationsFrom_WhenNoViolationsAreFound_ReturnsEmptyEnumerable()
        {
            // Arrange.
            var config = new ReferenceCopConfigBuilder().Build();
            var comparer = Substitute.For<IEqualityComparer<string>>();
            var detector = new AssemblyNameViolationDetector(comparer, config);
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
    }
}
