namespace ReferenceCop.MSBuild
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Build.Evaluation;

    /// <summary>
    /// Provides project reference information using MSBuild project evaluation.
    /// </summary>
    public class MSBuildProjectMetadataProvider : IProjectMetadataProvider
    {
        private const string ProjectReferenceNode = "ProjectReference";
        private const string NoWarnMetadata = "NoWarn";

        /// <summary>
        /// Gets the project references from a project file.
        /// </summary>
        /// <param name="projectFilePath">The path to the project file.</param>
        /// <returns>The collection of project references.</returns>
        public IEnumerable<ProjectReferenceInfo> GetProjectReferences(string projectFilePath)
        {
            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(projectFilePath);

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
            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(projectFilePath);
            project.ReevaluateIfNecessary();

            return project.GetPropertyValue(propertyName);
        }
    }
}
