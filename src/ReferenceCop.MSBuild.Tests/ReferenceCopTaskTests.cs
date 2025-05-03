namespace ReferenceCop.MSBuild.Tests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.Build.Framework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;
    using ReferenceCop;

    [TestClass]
    public class ReferenceCopTaskTests
    {
        [TestMethod]
        public void Execute_WhenViolationHasWarningSeverity_ReturnsTrue()
        {
            // Arrange.
            var fakeBuildEngine = Substitute.For<IBuildEngine>();
            var tagViolationDetector = Substitute.For<IViolationDetector<string>>();
            var pathViolationDetector = Substitute.For<IViolationDetector<string>>();
            var fakeProjectReferencesProvider = Substitute.For<IProjectMetadataProvider>();
            var fakeConfigLoader = Substitute.For<IConfigurationLoader>();
            var taskItem = Substitute.For<ITaskItem>();
            taskItem.ItemSpec.Returns("TestProject.csproj");

            var rule = new ReferenceCopConfig.AssemblyName { Severity = ReferenceCopConfig.Rule.ViolationSeverity.Warning };
            var warningViolation = new Violation(rule, "TestReference");

            // Setup the fake IProjectMetadataProvider
            fakeProjectReferencesProvider.GetProjectReferences(Arg.Any<string>())
                .Returns(new List<string> { "ReferenceProject.csproj" });
            fakeProjectReferencesProvider.GetPropertyValue(Arg.Any<string>(), Arg.Any<string>())
                .Returns("C:\\path\\to\\repo");

            tagViolationDetector.GetViolationsFrom(Arg.Any<IEnumerable<string>>())
                .Returns(new List<Violation> { warningViolation });
            pathViolationDetector.GetViolationsFrom(Arg.Any<IEnumerable<string>>())
                .Returns(new List<Violation>());

            fakeConfigLoader.Load().Returns(new ReferenceCopConfig());

            var task = new ReferenceCopTask(fakeProjectReferencesProvider, fakeConfigLoader, tagViolationDetector, pathViolationDetector)
            {
                BuildEngine = fakeBuildEngine,
                ProjectFile = taskItem,
                ConfigFilePaths = "C:\\path\\to\\config.xml",
            };

            // Act.
            bool result = task.Execute();

            // Assert.
            result.Should().BeTrue();

            // Verify that LogViolation was called with the warning violation
            fakeBuildEngine.Received().LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
            fakeBuildEngine.DidNotReceive().LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
        }

        [TestMethod]
        public void Execute_WhenViolationHasErrorSeverity_ReturnsFalse()
        {
            // Arrange.
            var fakeBuildEngine = Substitute.For<IBuildEngine>();
            var taskItem = Substitute.For<ITaskItem>();
            taskItem.ItemSpec.Returns("TestProject.csproj");

            var tagViolationDetector = Substitute.For<IViolationDetector<string>>();
            var pathViolationDetector = Substitute.For<IViolationDetector<string>>();
            var fakeProjectReferencesProvider = Substitute.For<IProjectMetadataProvider>();
            var fakeConfigLoader = Substitute.For<IConfigurationLoader>();

            // Setup the fake IProjectReferencesProvider
            fakeProjectReferencesProvider.GetProjectReferences(Arg.Any<string>())
                .Returns(new List<string> { "ReferenceProject.csproj" });
            fakeProjectReferencesProvider.GetPropertyValue(Arg.Any<string>(), Arg.Any<string>())
                .Returns("C:\\path\\to\\repo");

            var rule = new ReferenceCopConfig.AssemblyName { Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error };
            var errorViolation = new Violation(rule, "TestReference");

            tagViolationDetector.GetViolationsFrom(Arg.Any<IEnumerable<string>>())
                .Returns(new List<Violation> { errorViolation });
            pathViolationDetector.GetViolationsFrom(Arg.Any<IEnumerable<string>>())
                .Returns(new List<Violation>());

            fakeConfigLoader.Load().Returns(new ReferenceCopConfig());

            var task = new ReferenceCopTask(fakeProjectReferencesProvider, fakeConfigLoader, tagViolationDetector, pathViolationDetector)
            {
                BuildEngine = fakeBuildEngine,
                ProjectFile = taskItem,
                ConfigFilePaths = "C:\\path\\to\\config.xml",
            };

            // Act
            bool result = task.Execute();

            // Assert
            result.Should().BeFalse();

            // Verify that LogViolation was called with the error violation
            fakeBuildEngine.Received().LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
            fakeBuildEngine.DidNotReceive().LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
        }

        [TestMethod]
        public void Execute_WhenHasBothErrorAndWarningSeverities_ReturnsFalse()
        {
            // Arrange
            var fakeBuildEngine = Substitute.For<IBuildEngine>();
            var taskItem = Substitute.For<ITaskItem>();
            taskItem.ItemSpec.Returns("TestProject.csproj");

            var tagViolationDetector = Substitute.For<IViolationDetector<string>>();
            var pathViolationDetector = Substitute.For<IViolationDetector<string>>();
            var fakeProjectReferencesProvider = Substitute.For<IProjectMetadataProvider>();
            var fakeConfigLoader = Substitute.For<IConfigurationLoader>();

            // Setup the fake IProjectReferencesProvider
            fakeProjectReferencesProvider.GetProjectReferences(Arg.Any<string>())
                .Returns(new List<string> { "ReferenceProject.csproj" });
            fakeProjectReferencesProvider.GetPropertyValue(Arg.Any<string>(), Arg.Any<string>())
                .Returns("C:\\path\\to\\repo");

            var errorRule = new ReferenceCopConfig.AssemblyName { Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error };
            var warningRule = new ReferenceCopConfig.AssemblyName { Severity = ReferenceCopConfig.Rule.ViolationSeverity.Warning };
            var errorViolation = new Violation(errorRule, "TestErrorReference");
            var warningViolation = new Violation(warningRule, "TestWarningReference");

            tagViolationDetector.GetViolationsFrom(Arg.Any<IEnumerable<string>>())
                .Returns(new List<Violation> { errorViolation });
            pathViolationDetector.GetViolationsFrom(Arg.Any<IEnumerable<string>>())
                .Returns(new List<Violation> { warningViolation });

            fakeConfigLoader.Load().Returns(new ReferenceCopConfig());

            var task = new ReferenceCopTask(fakeProjectReferencesProvider, fakeConfigLoader, tagViolationDetector, pathViolationDetector)
            {
                BuildEngine = fakeBuildEngine,
                ProjectFile = taskItem,
                ConfigFilePaths = "C:\\path\\to\\config.xml",
            };

            // Act
            bool result = task.Execute();

            // Assert
            result.Should().BeFalse();

            // Verify that LogViolation was called with both violations
            fakeBuildEngine.Received().LogErrorEvent(Arg.Any<BuildErrorEventArgs>());
            fakeBuildEngine.Received().LogWarningEvent(Arg.Any<BuildWarningEventArgs>());
        }
    }
}
