namespace ReferenceCop.Tests
{
    using System;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AssemblyPathProviderTests
    {
        [TestMethod]
        public void GetAssemblyPath_WhenRepositoryRootIsNull_ThrowsArgumentNullException()
        {
            // Arrange.
            string nullRepositoryRoot = null;

            // Act.
            Action act = () => new AssemblyPathProvider(nullRepositoryRoot);
            

            // Assert.
            act.Should().Throw<ArgumentNullException>().WithMessage("*repositoryRoot*");
        }

        [TestMethod]
        public void GetAssemblyPath_WhenProjectFilePathIsNull_ThrowsArgumentNullException()
        {
            // Arrange.
            var provider = new AssemblyPathProvider(@"C:\Repo\Project");
            string nullProjectFilePath = null;

            // Act.
            Action act = () => provider.GetAssemblyPath(nullProjectFilePath);

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
        public void GetAssemblyPath_ReturnsPathRelativeToRepositoryRoot(string repositoryRoot, string projectFilePath, string expectedAssemblyPath)
        {
            // Arrange.
            var provider = new AssemblyPathProvider(repositoryRoot);

            // Act.
            var result = provider.GetAssemblyPath(projectFilePath);

            // Assert.
            result.Should().Be(expectedAssemblyPath);
        }
    }
}
