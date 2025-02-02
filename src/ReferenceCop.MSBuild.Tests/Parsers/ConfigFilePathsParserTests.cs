namespace ReferenceCop.MSBuild.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ConfigFilePathsParserTests
    {
        private const string ValidPath = "C:\\config\\file1.config";
        private const string AnotherValidPath = "C:\\config\\file2.config";
        private const string EmptyPath = "   ";
        private const string NullPath = null;

        [TestMethod]
        public void Parse_WhenConfigFilePathsIsNull_ThrowsArgumentException()
        {
            // Arrange.
            string configFilePaths = NullPath;

            // Act.
            Action act = () => ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Parse_WhenConfigFilePathsIsEmpty_ThrowsArgumentException()
        {
            // Arrange.
            string configFilePaths = string.Empty;

            // Act.
            Action act = () => ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            act.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Parse_WhenConfigFilePathsContainsOnlyEmptyEntries_ThrowsInvalidOperationException()
        {
            // Arrange.
            string configFilePaths = $"{EmptyPath};{EmptyPath}";

            // Act.
            Action act = () => ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Parse_WhenConfigFilePathsContainsMultipleValidPaths_ThrowsInvalidOperationException()
        {
            // Arrange.
            string configFilePaths = $"{ValidPath};{AnotherValidPath}";

            // Act.
            Action act = () => ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void Parse_WhenConfigFilePathsContainsSingleValidPath_ReturnsValidPath()
        {
            // Arrange.
            string configFilePaths = ValidPath;

            // Act.
            string result = ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            result.Should().Be(ValidPath);
        }

        [TestMethod]
        public void Parse_WhenConfigFilePathsContainsValidPathWithWhitespace_ReturnsTrimmedValidPath()
        {
            // Arrange.
            string configFilePaths = $"  {ValidPath}  ";

            // Act.
            string result = ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            result.Should().Be(ValidPath);
        }

        [TestMethod]
        public void Parse_WhenConfigFilePathsContainsValidAndEmptyPaths_ReturnsValidPath()
        {
            // Arrange.
            string configFilePaths = $"{EmptyPath};{ValidPath};{EmptyPath}";

            // Act.
            string result = ConfigFilePathsParser.Parse(configFilePaths);

            // Assert.
            result.Should().Be(ValidPath);
        }
    }
}
