namespace ReferenceCop.MSBuild
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides project metadata from csproj files.
    /// </summary>
    public interface IProjectMetadataProvider
    {
        /// <summary>
        /// Gets the project references from a project file.
        /// </summary>
        /// <param name="projectFilePath">The path to the project file.</param>
        /// <returns>The collection of project references.</returns>
        IEnumerable<string> GetProjectReferences(string projectFilePath);

        /// <summary>
        /// Gets a resolved property value from a project file.
        /// </summary>
        /// <param name="projectFilePath">The path to the project file.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The resolved property value.</returns>
        string GetPropertyValue(string projectFilePath, string propertyName);
    }
}
