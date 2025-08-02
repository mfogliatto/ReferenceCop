namespace ReferenceCop.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class ProjectPathViolationDetectorTests
    {
        private const string ProjectOneDir = @"C:\Repo\ProjectOne\";
        private const string ProjectTwoDir = @"C:\Repo\ProjectTwo\";
        private const string ProjectThreeDir = @"C:\Repo\ProjectThree\";
        private string projectOneFilePath = $"{ProjectOneDir}Project.csproj";
        private string projectTwoFilePath = $"{ProjectTwoDir}ProjectTwo.csproj";
        private string projectThreeFilePath = $"{ProjectThreeDir}ProjectThree.csproj";

        [TestMethod]
        public void GetViolationsFrom_WhenNoRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig();
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, Substitute.For<IProjectPathProvider>());
            var references = new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(this.projectOneFilePath),
            };

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
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, Substitute.For<IProjectPathProvider>());

            // Act.
            var result = detector.GetViolationsFrom(Enumerable.Empty<ReferenceEvaluationContext<string>>());

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
                    new ReferenceCopConfig.ProjectPath { FromPath = @"C:\OtherRepo", ToPath = ProjectOneDir },
                },
            };
            var projectPathProvider = Substitute.For<IProjectPathProvider>();
            projectPathProvider.GetRelativePath(this.projectOneFilePath).Returns(ProjectOneDir);
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, projectPathProvider);
            var references = new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(this.projectTwoFilePath),
            };

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
                    new ReferenceCopConfig.ProjectPath { FromPath = ProjectOneDir, ToPath = ProjectTwoDir },
                },
            };
            var projectPathProvider = Substitute.For<IProjectPathProvider>();
            projectPathProvider.GetRelativePath(this.projectOneFilePath).Returns(ProjectOneDir);
            projectPathProvider.GetRelativePath(this.projectTwoFilePath).Returns(ProjectTwoDir);
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, projectPathProvider);
            var references = new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(this.projectTwoFilePath),
            };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().ContainSingle();
            result.Should().Contain(v => v.ReferenceName == this.projectTwoFilePath);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMultipleMatchingReferences_ReturnsViolations()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                {
                    new ReferenceCopConfig.ProjectPath { FromPath = ProjectOneDir, ToPath = ProjectTwoDir },
                    new ReferenceCopConfig.ProjectPath { FromPath = ProjectOneDir, ToPath = ProjectThreeDir },
                },
            };
            var projectPathProvider = Substitute.For<IProjectPathProvider>();
            projectPathProvider.GetRelativePath(this.projectOneFilePath).Returns(ProjectOneDir);
            projectPathProvider.GetRelativePath(this.projectTwoFilePath).Returns(ProjectTwoDir);
            projectPathProvider.GetRelativePath(this.projectThreeFilePath).Returns(ProjectThreeDir);
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, projectPathProvider);
            var references = new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(this.projectTwoFilePath),
                ReferenceEvaluationContextFactory.Create(this.projectThreeFilePath),
            };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().HaveCount(2);
            result.Should().Contain(v => v.ReferenceName == this.projectTwoFilePath);
            result.Should().Contain(v => v.ReferenceName == this.projectThreeFilePath);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenWarningSuppressed_SkipsViolation()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                {
                    new ReferenceCopConfig.ProjectPath { FromPath = ProjectOneDir, ToPath = ProjectTwoDir },
                },
            };
            var projectPathProvider = Substitute.For<IProjectPathProvider>();
            projectPathProvider.GetRelativePath(this.projectOneFilePath).Returns(ProjectOneDir);
            projectPathProvider.GetRelativePath(this.projectTwoFilePath).Returns(ProjectTwoDir);
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, projectPathProvider);
            var references = new List<ReferenceEvaluationContext<string>>
            {
                // Create with warning suppressed
                new ReferenceEvaluationContext<string>(this.projectTwoFilePath, isWarningSuppressed: true),
            };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().BeEmpty("because the violation was suppressed");
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMultipleReferencesWithSomeSuppressed_ReturnsSomeViolations()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                {
                    new ReferenceCopConfig.ProjectPath { FromPath = ProjectOneDir, ToPath = ProjectTwoDir },
                    new ReferenceCopConfig.ProjectPath { FromPath = ProjectOneDir, ToPath = ProjectThreeDir },
                },
            };
            var projectPathProvider = Substitute.For<IProjectPathProvider>();
            projectPathProvider.GetRelativePath(this.projectOneFilePath).Returns(ProjectOneDir);
            projectPathProvider.GetRelativePath(this.projectTwoFilePath).Returns(ProjectTwoDir);
            projectPathProvider.GetRelativePath(this.projectThreeFilePath).Returns(ProjectThreeDir);
            var detector = new ProjectPathViolationDetector(config, this.projectOneFilePath, projectPathProvider);
            var references = new List<ReferenceEvaluationContext<string>>
            {
                // Regular reference with no suppression
                new ReferenceEvaluationContext<string>(this.projectTwoFilePath, isWarningSuppressed: false),

                // Suppressed reference
                new ReferenceEvaluationContext<string>(this.projectThreeFilePath, isWarningSuppressed: true),
            };

            // Act.
            var result = detector.GetViolationsFrom(references);

            // Assert.
            result.Should().ContainSingle();
            result.Should().Contain(v => v.ReferenceName == this.projectTwoFilePath);
            result.Should().NotContain(v => v.ReferenceName == this.projectThreeFilePath);
        }
    }
}
