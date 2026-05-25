namespace ReferenceCop
{
    using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<string, string> _cache = new();

        public string GetProjectTag(string projectFilePath)
        {
            return _cache.GetOrAdd(projectFilePath, path =>
            {
                if (!File.Exists(path))
                {
                    return UnknownProjectTag;
                }

                var projectFile = XDocument.Load(path);
                var projectTag = projectFile
                    .Descendants(PropertyGroupNode)
                    .Elements(ProjectTagNode)
                    .FirstOrDefault()?.Value;

                return projectTag ?? UnknownProjectTag;
            });
        }
    }
}
