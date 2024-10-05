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
        public void GetViolationsFrom_WhenViolationIsFound_ReturnsDiagnosticEntry()
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
    }
}
