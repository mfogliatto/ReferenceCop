namespace ReferenceCop.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProjectPathProviderTests
    {
        [TestMethod]
        public void GetRelativePath_WhenRepositoryRootIsNull_ThrowsArgumentNullException()
        {
            // Arrange.
            string nullRepositoryRoot = null;

            // Act.
            Action act = () => new ProjectPathProvider(nullRepositoryRoot);
            

            // Assert.
            act.Should().Throw<ArgumentNullException>().WithMessage("*repositoryRoot*");
        }

        [TestMethod]
        public void GetRelativePath_WhenProjectFilePathIsNull_ThrowsArgumentNullException()
        {
            // Arrange.
            var provider = new ProjectPathProvider(@"C:\Repo\Project");
            string nullProjectFilePath = null;

            // Act.
            Action act = () => provider.GetRelativePath(nullProjectFilePath);

            // Assert.
            act.Should().Throw<ArgumentNullException>().WithMessage("*projectFilePath*");
        }

        [TestMethod]
        [DataRow(@"C:\Repo", @"C:\Repo\Project\Project.csproj", @"Project\Project.csproj")]
        [DataRow(@"C:\Repo\", @"C:\Repo\Project\Project.csproj", @"Project\Project.csproj")]
        [DataRow(@"C:\Repo", @"C:\Repo\Project\SubDir\Project.csproj", @"Project\SubDir\Project.csproj")]
        [DataRow(@"C:\Repo\", @"C:\Repo\Project\SubDir\Project.csproj", @"Project\SubDir\Project.csproj")]
        [DataRow(@"C:\Repo\Project", @"C:\Repo\AnotherProject\Project.csproj", @"..\AnotherProject\Project.csproj")]
        [DataRow(@"C:\Repo\Project\", @"C:\Repo\AnotherProject\Project.csproj", @"..\AnotherProject\Project.csproj")]
        [DataRow(@"C:\Repo", @"D:\AnotherRepo\Project\Project.csproj", @"D:\AnotherRepo\Project\Project.csproj")]
        [DataRow(@"C:\Repo\", @"D:\AnotherRepo\Project\Project.csproj", @"D:\AnotherRepo\Project\Project.csproj")]
        public void GetRelativePath_ReturnsPathRelativeToRepositoryRoot(string repositoryRoot, string projectFilePath, string expectedRelativePath)
        {
            // Arrange.
            var provider = new ProjectPathProvider(repositoryRoot);

            // Act.
            var result = provider.GetRelativePath(projectFilePath);

            // Assert.
            result.Should().Be(expectedRelativePath);
        }
    }
}
