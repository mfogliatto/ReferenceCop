namespace ReferenceCop.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class AssemblyPathViolationDetectorTests
    {
        private const string ProjectOneDir = @"C:\Repo\ProjectOne\";
        private string ProjectOneFilePath = $"{ProjectOneDir}Project.csproj";
        private const string ProjectTwoDir = @"C:\Repo\ProjectTwo\";
        private string ProjectTwoFilePath = $"{ProjectTwoDir}ProjectTwo.csproj";
        private const string ProjectThreeDir = @"C:\Repo\ProjectThree\";
        private string ProjectThreeFilePath = $"{ProjectThreeDir}ProjectThree.csproj";

        [TestMethod]
        public void GetViolationsFrom_WhenNoRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig();
            var detector = new AssemblyPathViolationDetector(config, ProjectOneFilePath, Substitute.For<IAssemblyPathProvider>());
            var references = new List<string> { ProjectOneFilePath };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoReferences_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig();
            var detector = new AssemblyPathViolationDetector(config, ProjectOneFilePath, Substitute.For<IAssemblyPathProvider>());

            // Act.
            var result = detector.GetViolationsFrom(Enumerable.Empty<string>());

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoMatchingRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                {
                    new ReferenceCopConfig.AssemblyPath { FromPath = @"C:\OtherRepo", ToPath = ProjectOneDir }
                }
            };
            var assemblyPathProvider = Substitute.For<IAssemblyPathProvider>();
            assemblyPathProvider.GetAssemblyPath(ProjectOneFilePath).Returns(ProjectOneDir);
            var detector = new AssemblyPathViolationDetector(config, ProjectOneFilePath, assemblyPathProvider);
            var references = new List<string> { ProjectTwoFilePath };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMatchingRule_ReturnsViolations()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                {
                    new ReferenceCopConfig.AssemblyPath { FromPath = ProjectOneDir, ToPath = ProjectTwoDir }
                }
            };
            var assemblyPathProvider = Substitute.For<IAssemblyPathProvider>();
            assemblyPathProvider.GetAssemblyPath(ProjectOneFilePath).Returns(ProjectOneDir);
            assemblyPathProvider.GetAssemblyPath(ProjectTwoFilePath).Returns(ProjectTwoDir);
            var detector = new AssemblyPathViolationDetector(config, ProjectOneFilePath, assemblyPathProvider);
            var references = new List<string> { ProjectTwoFilePath };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().ContainSingle();
            result.Should().Contain(v => v.ReferenceName == ProjectTwoFilePath);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMultipleMatchingReferences_ReturnsViolations()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                {
                    new ReferenceCopConfig.AssemblyPath { FromPath = ProjectOneDir, ToPath = ProjectTwoDir },
                    new ReferenceCopConfig.AssemblyPath { FromPath = ProjectOneDir, ToPath = ProjectThreeDir }
                }
            };
            var assemblyPathProvider = Substitute.For<IAssemblyPathProvider>();
            assemblyPathProvider.GetAssemblyPath(ProjectOneFilePath).Returns(ProjectOneDir);
            assemblyPathProvider.GetAssemblyPath(ProjectTwoFilePath).Returns(ProjectTwoDir);
            assemblyPathProvider.GetAssemblyPath(ProjectThreeFilePath).Returns(ProjectThreeDir);
            var detector = new AssemblyPathViolationDetector(config, ProjectOneFilePath, assemblyPathProvider);
            var references = new List<string> { ProjectTwoFilePath, ProjectThreeFilePath };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().HaveCount(2);
            result.Should().Contain(v => v.ReferenceName == ProjectTwoFilePath);
            result.Should().Contain(v => v.ReferenceName == ProjectThreeFilePath);
        }
    }
}
