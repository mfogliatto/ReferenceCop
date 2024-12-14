namespace ReferenceCop
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    [ExcludeFromCodeCoverage]
    public class AssemblyTagProvider : IAssemblyTagProvider
    {
        internal const string PropertyGroupNode = "PropertyGroup";
        internal const string AssemblyTagNode = "AssemblyTag";
        internal const string UnknownAssemblyTag = "Unknown";

        public string GetAssemblyTag(string projectFilePath)
        {
            if (!File.Exists(projectFilePath))
            {
                return UnknownAssemblyTag;
            }

            var projectFile = XDocument.Load(projectFilePath);
            var assemblyTag = projectFile
                .Descendants(PropertyGroupNode)
                .Elements(AssemblyTagNode)
                .FirstOrDefault()?.Value;

            return assemblyTag ?? UnknownAssemblyTag;
        }
    }
}