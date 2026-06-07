namespace ReferenceCop.MSBuild
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Provides project reference information using MSBuild project evaluation.
    /// </summary>
    public class MSBuildProjectMetadataProvider : IProjectMetadataProvider, IDisposable
    {
        private const string ProjectReferenceNode = "ProjectReference";
        private const string NoWarnMetadata = "NoWarn";

        private readonly ProjectCollection projectCollection;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="MSBuildProjectMetadataProvider"/> class.
        /// </summary>
        public MSBuildProjectMetadataProvider()
        {
            this.projectCollection = new ProjectCollection();
        }

        /// <summary>
        /// Gets the project references from a project file.
        /// </summary>
        /// <param name="projectFilePath">The path to the project file.</param>
        /// <returns>The collection of project references.</returns>
        public IEnumerable<ProjectReferenceInfo> GetProjectReferences(string projectFilePath)
        {
            var project = this.LoadOrGetProject(projectFilePath);

            // Get all ProjectReference items. These are the direct project references.
            var projectReferences = project.GetItems(ProjectReferenceNode);

            // Extract the Include attribute and NoWarn metadata for each reference
            foreach (var pr in projectReferences)
            {
                string referencePath = pr.EvaluatedInclude;
                string noWarnValue = pr.GetMetadataValue(NoWarnMetadata);

                IEnumerable<string> noWarnCodes = string.IsNullOrEmpty(noWarnValue)
                    ? new List<string>()
                    : noWarnValue.Split(',').Select(code => code.Trim());

                yield return new ProjectReferenceInfo(referencePath, noWarnCodes);
            }
        }

        /// <summary>
        /// Gets a resolved property value from a project file.
        /// </summary>
        /// <param name="projectFilePath">The path to the project file.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The resolved property value.</returns>
        public string GetPropertyValue(string projectFilePath, string propertyName)
        {
            var project = this.LoadOrGetProject(projectFilePath);
            project.ReevaluateIfNecessary();

            return project.GetPropertyValue(propertyName);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources used by the <see cref="MSBuildProjectMetadataProvider"/>.
        /// </summary>
        /// <param name="disposing">Whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.projectCollection.UnloadAllProjects();
                    this.projectCollection.Dispose();
                }

                this.disposed = true;
            }
        }

        private Project LoadOrGetProject(string projectFilePath)
        {
            var fullPath = System.IO.Path.GetFullPath(projectFilePath);
            var loaded = this.projectCollection.GetLoadedProjects(fullPath);

            if (loaded.Count > 0)
            {
                return loaded.First();
            }

            return this.projectCollection.LoadProject(fullPath);
        }
    }
}
