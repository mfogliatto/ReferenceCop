namespace ReferenceCop
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    [ExcludeFromCodeCoverage]
    public class ProjectTagProvider : IProjectTagProvider
    {
        internal const string PropertyGroupNode = "PropertyGroup";
        internal const string ProjectTagNode = "ProjectTag";
        internal const string UnknownProjectTag = "Unknown";

        public string GetProjectTag(string projectFilePath)
        {
            if (!File.Exists(projectFilePath))
            {
                return UnknownProjectTag;
            }

            var projectFile = XDocument.Load(projectFilePath);
            var projectTag = projectFile
                .Descendants(PropertyGroupNode)
                .Elements(ProjectTagNode)
                .FirstOrDefault()?.Value;

            return projectTag ?? UnknownProjectTag;
        }
    }
}
