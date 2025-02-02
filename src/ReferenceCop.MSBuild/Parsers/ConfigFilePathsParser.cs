namespace ReferenceCop.MSBuild
{
    using System;
    using System.Linq;

    internal class ConfigFilePathsParser
    {
        internal static readonly char[] Separator = new[] { ';' };

        /// <summary>
        /// Parses a semi-colon separated string of config file paths and returns a single valid path.
        /// </summary>
        /// <param name="configFilePaths">A semi-colon separated string of config file paths.</param>
        /// <returns>A single valid config file path.</returns>
        /// <exception cref="ArgumentException">Thrown when no file paths are provided.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no valid paths are found or more than one path is provided.</exception>
        /// <remarks>
        /// This method is necessary because the AdditionalFiles property from MSBuild provides a string with multiple values separated by semi-colons.
        /// It ensures that only one valid config file path is returned, as the application does not support multiple config file paths.
        /// </remarks>
        public static string Parse(string configFilePaths)
        {
            if (string.IsNullOrEmpty(configFilePaths))
            {
                throw new ArgumentException("No file paths were provided.", nameof(configFilePaths));
            }

            var paths = configFilePaths.Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(path => path.Trim())
                                      .Where(path => !string.IsNullOrWhiteSpace(path))
                                      .ToList();

            if (paths.Count == 0)
            {
                throw new InvalidOperationException("No valid config file paths were found.");
            }

            if (paths.Count > 1)
            {
                throw new InvalidOperationException("More than one config file path is not supported.");
            }

            return paths.Single();
        }
    }
}
