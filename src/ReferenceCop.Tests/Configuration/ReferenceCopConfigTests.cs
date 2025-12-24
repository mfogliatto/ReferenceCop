namespace ReferenceCop.Tests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReferenceCopConfigTests
    {
        [TestMethod]
        public void Constructor_SetsDefaultValues()
        {
            // Act.
            var config = new ReferenceCopConfig();

            // Assert.
            config.Rules.Should().NotBeNull();
            config.Rules.Should().BeEmpty();
            config.UseExperimentalDetectors.Should().BeFalse("default value should be false for backward compatibility");
        }

        [TestMethod]
        public void UseExperimentalDetectors_CanBeSet()
        {
            // Arrange.
            var config = new ReferenceCopConfig();

            // Act.
            config.UseExperimentalDetectors = true;

            // Assert.
            config.UseExperimentalDetectors.Should().BeTrue();
        }
    }
}
