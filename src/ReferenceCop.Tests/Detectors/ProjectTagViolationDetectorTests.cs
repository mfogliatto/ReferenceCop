namespace ReferenceCop.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class ProjectTagViolationDetectorTests
    {
        private const string SourceFilePath = "testProject.csproj";
        private const string ReferenceFilePath1 = "reference1.csproj";
        private const string ReferenceFilePath2 = "reference2.csproj";
        private const string ProjectTag1 = "Tag1";
        private const string ProjectTag2 = "Tag2";
        private const string UnknownProjectTag = "Unknown";

        [TestMethod]
        public void GetViolationsFrom_WhenNoRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig();
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, Substitute.For<IProjectTagProvider>());

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath1),
            });

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoReferences_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>(),
            };
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, Substitute.For<IProjectTagProvider>());

            // Act.
            var result = detector.GetViolationsFrom(Enumerable.Empty<ReferenceEvaluationContext<string>>());

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoMatchingRules_ReturnsEmpty()
        {
            // Arrange.
            const string nonMatchingTag = "NonMatchingTag";
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.ProjectTag { FromProjectTag = nonMatchingTag, ToProjectTag = ProjectTag2 },
                            },
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.GetProjectTag(SourceFilePath).Returns(ProjectTag1);
            tagProvider.GetProjectTag(ReferenceFilePath1).Returns(nonMatchingTag);
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath1),
            });

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMatchingRule_ReturnsViolation()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.ProjectTag { FromProjectTag = ProjectTag1, ToProjectTag = ProjectTag2 },
                            },
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.GetProjectTag(SourceFilePath).Returns(ProjectTag1);
            tagProvider.GetProjectTag(ReferenceFilePath1).Returns(ProjectTag2);
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath1),
            });

            // Assert.
            result.Should().ContainSingle()
                .Which.Should().Match<Violation>(v => (v.Rule as ReferenceCopConfig.ProjectTag).FromProjectTag == ProjectTag1 &&
                                                      (v.Rule as ReferenceCopConfig.ProjectTag).ToProjectTag == ProjectTag2);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMultipleMatchingReferences_ReturnsViolations()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.ProjectTag { FromProjectTag = ProjectTag1, ToProjectTag = ProjectTag2 },
                            },
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.GetProjectTag(SourceFilePath).Returns(ProjectTag1);
            tagProvider.GetProjectTag(ReferenceFilePath1).Returns(ProjectTag2);
            tagProvider.GetProjectTag(ReferenceFilePath2).Returns(ProjectTag2);
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath1),
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath2),
            });

            // Assert.
            result.Should().HaveCount(2);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenUnknownProjectTag_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.ProjectTag { FromProjectTag = ProjectTag1, ToProjectTag = ProjectTag2 },
                            },
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.GetProjectTag(SourceFilePath).Returns(UnknownProjectTag);
            var detector = new ProjectTagViolationDetector(config, "nonexistent.csproj", tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath1),
            });

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenWarningSuppressed_SkipsViolation()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.ProjectTag { FromProjectTag = ProjectTag1, ToProjectTag = ProjectTag2 },
                            },
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.GetProjectTag(SourceFilePath).Returns(ProjectTag1);
            tagProvider.GetProjectTag(ReferenceFilePath1).Returns(ProjectTag2);
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                // Create with warning suppressed
                new ReferenceEvaluationContext<string>(ReferenceFilePath1, isWarningSuppressed: true),
            });

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
                                new ReferenceCopConfig.ProjectTag { FromProjectTag = ProjectTag1, ToProjectTag = ProjectTag2 },
                            },
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.GetProjectTag(SourceFilePath).Returns(ProjectTag1);
            tagProvider.GetProjectTag(ReferenceFilePath1).Returns(ProjectTag2);
            tagProvider.GetProjectTag(ReferenceFilePath2).Returns(ProjectTag2);
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                // Regular reference with no suppression
                new ReferenceEvaluationContext<string>(ReferenceFilePath1, isWarningSuppressed: false),

                // Suppressed reference
                new ReferenceEvaluationContext<string>(ReferenceFilePath2, isWarningSuppressed: true),
            });

            // Assert.
            result.Should().ContainSingle()
                .Which.ReferenceName.Should().Be(ReferenceFilePath1);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenTagProviderThrows_ReThrowsException()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>(),
            };
            var tagProvider = Substitute.For<IProjectTagProvider>();
            tagProvider.When(x => x.GetProjectTag(Arg.Any<string>())).Do(x => { throw new InvalidOperationException(); });
            var detector = new ProjectTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            Action act = () => detector.GetViolationsFrom(new List<ReferenceEvaluationContext<string>>
            {
                ReferenceEvaluationContextFactory.Create(ReferenceFilePath1),
            }).ToList();

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
