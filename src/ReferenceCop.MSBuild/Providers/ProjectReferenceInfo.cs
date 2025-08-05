namespace ReferenceCop.MSBuild
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains information about a project reference, including the path and NoWarn values.
    /// </summary>
    public class ProjectReferenceInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectReferenceInfo"/> class.
        /// </summary>
        /// <param name="path">The path to the referenced project.</param>
        /// <param name="noWarn">The NoWarn values for this reference.</param>
        public ProjectReferenceInfo(string path, IEnumerable<string> noWarn = null)
        {
            this.Path = path;
            this.NoWarn = noWarn ?? new List<string>();
        }

        /// <summary>
        /// Gets the path to the referenced project.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the NoWarn values for this reference.
        /// </summary>
        public IEnumerable<string> NoWarn { get; }
    }
}
