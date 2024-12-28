namespace ReferenceCop.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class AssemblyTagViolationDetectorTests
    {
        private const string SourceFilePath = "testProject.csproj";
        private const string ReferenceFilePath1 = "reference1.csproj";
        private const string ReferenceFilePath2 = "reference2.csproj";
        private const string AssemblyTag1 = "Tag1";
        private const string AssemblyTag2 = "Tag2";
        private const string UnknownAssemblyTag = "Unknown";

        [TestMethod]
        public void GetViolationsFrom_WhenNoRules_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig();
            var detector = new AssemblyTagViolationDetector(config, SourceFilePath, Substitute.For<IAssemblyTagProvider>());

            // Act.
            var result = detector.GetViolationsFrom(new List<string> { ReferenceFilePath1 });

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenNoReferences_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>()
            };
            var detector = new AssemblyTagViolationDetector(config, SourceFilePath, Substitute.For<IAssemblyTagProvider>());

            // Act.
            var result = detector.GetViolationsFrom(Enumerable.Empty<string>());

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
                                new ReferenceCopConfig.AssemblyTag { FromAssemblyTag = nonMatchingTag, ToAssemblyTag = AssemblyTag2 }
                            }
            };
            var tagProvider = Substitute.For<IAssemblyTagProvider>();
            tagProvider.GetAssemblyTag(SourceFilePath).Returns(AssemblyTag1);
            tagProvider.GetAssemblyTag(ReferenceFilePath1).Returns(nonMatchingTag);
            var detector = new AssemblyTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<string> { ReferenceFilePath1 });

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
                                new ReferenceCopConfig.AssemblyTag { FromAssemblyTag = AssemblyTag1, ToAssemblyTag = AssemblyTag2 }
                            }
            };
            var tagProvider = Substitute.For<IAssemblyTagProvider>();
            tagProvider.GetAssemblyTag(SourceFilePath).Returns(AssemblyTag1);
            tagProvider.GetAssemblyTag(ReferenceFilePath1).Returns(AssemblyTag2);
            var detector = new AssemblyTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<string> { ReferenceFilePath1 });

            // Assert.
            result.Should().ContainSingle()
                .Which.Should().Match<Violation>(v => (v.Rule as ReferenceCopConfig.AssemblyTag).FromAssemblyTag == AssemblyTag1 &&
                                                      (v.Rule as ReferenceCopConfig.AssemblyTag).ToAssemblyTag == AssemblyTag2);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenMultipleMatchingReferences_ReturnsViolations()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.AssemblyTag { FromAssemblyTag = AssemblyTag1, ToAssemblyTag = AssemblyTag2 }
                            }
            };
            var tagProvider = Substitute.For<IAssemblyTagProvider>();
            tagProvider.GetAssemblyTag(SourceFilePath).Returns(AssemblyTag1);
            tagProvider.GetAssemblyTag(ReferenceFilePath1).Returns(AssemblyTag2);
            tagProvider.GetAssemblyTag(ReferenceFilePath2).Returns(AssemblyTag2);
            var detector = new AssemblyTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<string> { ReferenceFilePath1, ReferenceFilePath2 });

            // Assert.
            result.Should().HaveCount(2);
        }

        [TestMethod]
        public void GetViolationsFrom_WhenUnknownAssemblyTag_ReturnsEmpty()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>
                            {
                                new ReferenceCopConfig.AssemblyTag { FromAssemblyTag = AssemblyTag1, ToAssemblyTag = AssemblyTag2 }
                            }
            };
            var tagProvider = Substitute.For<IAssemblyTagProvider>();
            tagProvider.GetAssemblyTag(SourceFilePath).Returns(UnknownAssemblyTag);
            var detector = new AssemblyTagViolationDetector(config, "nonexistent.csproj", tagProvider);

            // Act.
            var result = detector.GetViolationsFrom(new List<string> { ReferenceFilePath1 });

            // Assert.
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenTagProviderThrows_ReThrowsException()
        {
            // Arrange.
            var config = new ReferenceCopConfig
            {
                Rules = new List<ReferenceCopConfig.Rule>()
            };
            var tagProvider = Substitute.For<IAssemblyTagProvider>();
            tagProvider.When(x => x.GetAssemblyTag(Arg.Any<string>())).Do(x => { throw new InvalidOperationException(); });
            var detector = new AssemblyTagViolationDetector(config, SourceFilePath, tagProvider);

            // Act.
            Action act = () => detector.GetViolationsFrom(new List<string> { ReferenceFilePath1 }).ToList();

            // Assert.
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
