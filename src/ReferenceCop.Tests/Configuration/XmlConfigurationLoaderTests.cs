namespace ReferenceCop.Tests
{
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Xml;

    [TestClass]
    public class XmlConfigurationLoaderTests
    {
        [TestMethod]
        public void Load_WhenNoDetectionRulesArePresent_ShouldNotFail()
        {
            // Arrange.
            var xml = @"<ReferenceCopConfig></ReferenceCopConfig>";
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            // Act.
            var loader = new XmlConfigurationLoader(xmlDocument);
            var config = loader.Load();

            // Assert.
            config.Should().NotBeNull();
        }

        [TestMethod]
        public void Load_WhenDetectionRulesAreEmpty_ShouldNotFail()
        {
            // Arrange.
            var xml = @"<ReferenceCopConfig>
                            <DetectionRules></DetectionRules>
                        </ReferenceCopConfig>";
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            // Act.
            var loader = new XmlConfigurationLoader(xmlDocument);
            var config = loader.Load();

            // Assert.
            config.Should().NotBeNull();
        }

        [TestMethod]
        public void Load_WhenAnAssemblyNameRuleIsPresent_ShouldAddToDetectionRules()
        {
            // Arrange.
            var xml = @"<ReferenceCopConfig>
                            <Rules>
                                <AssemblyName>
                                    <Name>DoNotUse-X</Name>
                                    <Description>Use of X is forbidden. Please use Y instead</Description>       
                                    <Severity>Error</Severity>
                                    <Pattern>X</Pattern>
                                </AssemblyName>
                            </Rules>
                        </ReferenceCopConfig>";
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            // Act.
            var loader = new XmlConfigurationLoader(xmlDocument);
            var config = loader.Load();

            // Assert.
            using (new AssertionScope())
            {
                config.Should().NotBeNull();
                config.Rules.Should().HaveCount(1);
            }
        }
    }
}