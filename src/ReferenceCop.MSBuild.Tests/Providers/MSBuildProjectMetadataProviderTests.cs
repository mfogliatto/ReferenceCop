namespace ReferenceCop.MSBuild.Tests
{
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Build.Evaluation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MSBuildProjectMetadataProviderTests
    {
        private const string TestPropertyName = "TestProperty";
        private const string TestPropertyValue = "TestValue";
        private const string TestReference = "TestReference.csproj";

        private MSBuildProjectMetadataProvider provider;

        [TestInitialize]
        public void Setup()
        {
            // Arrange the provider for each test
            this.provider = new MSBuildProjectMetadataProvider();

            // Clean up any projects that might have been loaded in previous tests
            ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
        }

        [TestMethod]
        public void GetProjectReferences_WhenReferenceHasNoWarn_ReturnsNoWarnValue()
        {
            // Arrange.
            string tempProjectPath = this.CreateTempProjectFileWithReferenceNoWarn();

            try
            {
                // Act.
                var references = this.provider.GetProjectReferences(tempProjectPath).ToList();

                // Assert.
                references.Should().ContainSingle();
                var reference = references.Single();
                reference.Path.Should().Be("TestReferenceWithNoWarn.csproj");
                reference.NoWarn.Should().ContainSingle().Which.Should().Be("RC0001;RC0002");
            }
            finally
            {
                // Cleanup
                File.Delete(tempProjectPath);
            }
        }

        [TestMethod]
        public void GetProjectReferences_WhenProjectHasOneReference_ReturnsSingleReference()
        {
            // Arrange.
            string tempProjectPath = this.CreateTempProjectFileWithReferences();

            try
            {
                // Act.
                var references = this.provider.GetProjectReferences(tempProjectPath);

                // Assert.
                references.Should().NotBeNull().And.ContainSingle(r => r.Path == TestReference);
            }
            finally
            {
                // Cleanup
                File.Delete(tempProjectPath);
            }
        }

        [TestMethod]
        public void GetPropertyValue_WhenPropertyExists_ReturnsPropertyValue()
        {
            // Arrange.
            string tempProjectPath = this.CreateTempProjectFileWithProperty();

            try
            {
                // Act.
                string value = this.provider.GetPropertyValue(tempProjectPath, TestPropertyName);

                // Assert.
                value.Should().Be(TestPropertyValue);
            }
            finally
            {
                // Cleanup
                File.Delete(tempProjectPath);
            }
        }

        [TestMethod]
        public void GetPropertyValue_WhenPropertyDoesNotExist_ReturnsEmptyString()
        {
            // Arrange.
            string tempProjectPath = this.CreateTempProjectFileWithProperty();
            const string nonExistentProperty = "NonExistentProperty";

            try
            {
                // Act.
                string value = this.provider.GetPropertyValue(tempProjectPath, nonExistentProperty);

                // Assert.
                value.Should().BeEmpty();
            }
            finally
            {
                // Cleanup
                File.Delete(tempProjectPath);
            }
        }

        private string CreateTempProjectFileWithReferences()
        {
            string tempFile = Path.GetTempFileName() + ".csproj";
            string projectContent = $@"
<Project>
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""{TestReference}"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(tempFile, projectContent);
            return tempFile;
        }

        private string CreateTempProjectFileWithProperty()
        {
            string tempFile = Path.GetTempFileName() + ".csproj";
            string projectContent = $@"
<Project>
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <{TestPropertyName}>{TestPropertyValue}</{TestPropertyName}>
  </PropertyGroup>
</Project>";
            File.WriteAllText(tempFile, projectContent);
            return tempFile;
        }

        private string CreateTempProjectFileWithReferenceNoWarn()
        {
            string tempFile = Path.GetTempFileName() + ".csproj";
            string projectContent = @"<Project>
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include=""TestReferenceWithNoWarn.csproj"" NoWarn=""RC0001;RC0002"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(tempFile, projectContent);
            return tempFile;
        }
    }
}
