namespace ReferenceCop
{
    using System;
    using System.IO;

    public class AssemblyPathProvider : IAssemblyPathProvider
    {
        private readonly string repositoryRoot;

        public AssemblyPathProvider(string repositoryRoot)
        {
            if (repositoryRoot == null) throw new ArgumentNullException(nameof(repositoryRoot));

            this.repositoryRoot = repositoryRoot;
        }

        /// <summary>
        /// Gets the relative path of the project file path from the repository root.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetAssemblyPath(string projectFilePath)
        {
            if (projectFilePath == null) throw new ArgumentNullException(nameof(projectFilePath));

            string relativeTo = this.repositoryRoot;
            relativeTo = Path.GetFullPath(relativeTo).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            projectFilePath = Path.GetFullPath(projectFilePath);

            Uri relativeToUri = new Uri(relativeTo);
            Uri projectFilePathUri = new Uri(projectFilePath);

            Uri relativeUri = relativeToUri.MakeRelativeUri(projectFilePathUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }
    }
}
