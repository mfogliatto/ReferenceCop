namespace ReferenceCop.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProjectExceptionTests
    {
        [TestMethod]
        public void IsProjectExempt_WhenProjectInExceptions_ReturnsTrue()
        {
            // Arrange.
            var rule = new ReferenceCopConfig.AssemblyName
            {
                Name = "TestRule",
                Pattern = "System.Xml",
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Exceptions = new List<ReferenceCopConfig.ProjectException>
                {
                    new ReferenceCopConfig.ProjectException { Name = "MyProject" },
                },
            };

            // Act & Assert.
            rule.IsProjectExempt("MyProject").Should().BeTrue();
        }

        [TestMethod]
        public void IsProjectExempt_WhenProjectNotInExceptions_ReturnsFalse()
        {
            // Arrange.
            var rule = new ReferenceCopConfig.AssemblyName
            {
                Name = "TestRule",
                Pattern = "System.Xml",
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Exceptions = new List<ReferenceCopConfig.ProjectException>
                {
                    new ReferenceCopConfig.ProjectException { Name = "OtherProject" },
                },
            };

            // Act & Assert.
            rule.IsProjectExempt("MyProject").Should().BeFalse();
        }

        [TestMethod]
        public void IsProjectExempt_WhenNoExceptions_ReturnsFalse()
        {
            // Arrange.
            var rule = new ReferenceCopConfig.AssemblyName
            {
                Name = "TestRule",
                Pattern = "System.Xml",
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
            };

            // Act & Assert.
            rule.IsProjectExempt("MyProject").Should().BeFalse();
        }

        [TestMethod]
        public void IsProjectExempt_IsCaseInsensitive()
        {
            // Arrange.
            var rule = new ReferenceCopConfig.AssemblyName
            {
                Name = "TestRule",
                Pattern = "System.Xml",
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Exceptions = new List<ReferenceCopConfig.ProjectException>
                {
                    new ReferenceCopConfig.ProjectException { Name = "MyProject" },
                },
            };

            // Act & Assert.
            rule.IsProjectExempt("myproject").Should().BeTrue();
            rule.IsProjectExempt("MYPROJECT").Should().BeTrue();
        }

        [TestMethod]
        public void GetViolationsFrom_WhenProjectIsExempt_DoesNotReportViolation()
        {
            // Arrange.
            const string pattern = "System.Xml";
            var config = new ReferenceCopConfig();
            config.Rules.Add(new ReferenceCopConfig.AssemblyName
            {
                Name = "NoXml",
                Pattern = pattern,
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Exceptions = new List<ReferenceCopConfig.ProjectException>
                {
                    new ReferenceCopConfig.ProjectException { Name = "ExemptProject" },
                },
            });

            var detector = new AssemblyNameViolationDetector(new ExactMatchComparer(), config, "ExemptProject");
            var references = new[]
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(pattern)),
            };

            // Act.
            var violations = detector.GetViolationsFrom(references).ToList();

            // Assert.
            violations.Should().BeEmpty("because ExemptProject is in the rule's exceptions");
        }

        [TestMethod]
        public void GetViolationsFrom_WhenProjectIsNotExempt_ReportsViolation()
        {
            // Arrange.
            const string pattern = "System.Xml";
            var config = new ReferenceCopConfig();
            config.Rules.Add(new ReferenceCopConfig.AssemblyName
            {
                Name = "NoXml",
                Pattern = pattern,
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Exceptions = new List<ReferenceCopConfig.ProjectException>
                {
                    new ReferenceCopConfig.ProjectException { Name = "OtherProject" },
                },
            });

            var detector = new AssemblyNameViolationDetector(new ExactMatchComparer(), config, "NonExemptProject");
            var references = new[]
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(pattern)),
            };

            // Act.
            var violations = detector.GetViolationsFrom(references).ToList();

            // Assert.
            violations.Should().HaveCount(1);
        }

        [TestMethod]
        public void GetViolationsFromExperimental_WhenProjectIsExempt_DoesNotReportViolation()
        {
            // Arrange.
            const string pattern = "System.Xml";
            var config = new ReferenceCopConfig();
            config.Rules.Add(new ReferenceCopConfig.AssemblyName
            {
                Name = "NoXml",
                Pattern = pattern,
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Exceptions = new List<ReferenceCopConfig.ProjectException>
                {
                    new ReferenceCopConfig.ProjectException { Name = "ExemptProject" },
                },
            });

            var detector = new AssemblyNameViolationDetector(new ExactMatchComparer(), config, "ExemptProject");
            var references = new[]
            {
                ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(pattern)),
            };

            // Act.
            var violations = detector.GetViolationsFromExperimental(references).ToList();

            // Assert.
            violations.Should().BeEmpty("because ExemptProject is in the rule's exceptions");
        }
    }
}
