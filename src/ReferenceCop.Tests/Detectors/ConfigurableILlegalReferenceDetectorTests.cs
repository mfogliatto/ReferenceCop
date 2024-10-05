namespace ReferenceCop.Tests.Detectors
{
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using System.Xml;

    [TestClass]
    public class ConfigurableILlegalReferenceDetectorTests
    {
        [TestMethod]
        public void GetIllegalReferencesFrom_WhenConfigContainsRuleForAssembly_ReturnsDiagnostic()
        {
            // Arrange.
            var documentXml = @"<Rules>
                                    <Rule>
                                        <Assembly>System.Xml</Assembly>
                                        <Message>System.Xml is not allowed</Message>
                                    </Rule>
                                </Rules>";
                        var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(documentXml);

            var detector = new ConfigurableILlegalReferenceDetector(xmlDocument);
            var references = new[]
            {
                new AssemblyIdentity("System.Xml"),
                new AssemblyIdentity("System.Xml.Serialization"),
                new AssemblyIdentity("System.Xml.Linq"),
            };

            // Act
            var diagnostics = detector.GetIllegalReferencesFrom(references);

            // Assert
            diagnostics.Should().HaveCount(2);
        }
    }
}
