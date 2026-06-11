namespace ReferenceCop
{
    using System;
    using System.IO;

    public class ProjectPathProvider : IProjectPathProvider
    {
        private readonly Uri repositoryRootUri;

        public ProjectPathProvider(string repositoryRoot)
        {
            if (repositoryRoot == null)
            {
                throw new ArgumentNullException(nameof(repositoryRoot));
            }

            string normalizedRoot = Path.GetFullPath(repositoryRoot)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
            this.repositoryRootUri = new Uri(normalizedRoot);
        }

        /// <summary>
        /// Gets the relative path of the project file path from the repository root.
        /// </summary>
        /// <exception cref="ArgumentNullException">string.</exception>
        public string GetRelativePath(string projectFilePath)
        {
            if (projectFilePath == null)
            {
                throw new ArgumentNullException(nameof(projectFilePath));
            }

            projectFilePath = Path.GetFullPath(projectFilePath);

            Uri projectFilePathUri = new Uri(projectFilePath);

            Uri relativeUri = this.repositoryRootUri.MakeRelativeUri(projectFilePathUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            return relativePath;
        }
    }
}
