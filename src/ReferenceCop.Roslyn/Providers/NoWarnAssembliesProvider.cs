namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provider for NoWarn assembly configurations in ReferenceCop.
    /// </summary>
    public class NoWarnAssembliesProvider : INoWarnAssembliesProvider
    {
        private static readonly Dictionary<string, IEnumerable<string>> EmptyDictionary = new Dictionary<string, IEnumerable<string>>();

        private static readonly char[] AssemblyEntriesSeparator = new[] { ';' };
        private static readonly char[] EntryPartsSeparator = new[] { '|' };
        private static readonly char[] NoWarnCodesSeparator = new[] { ',' };

        /// <summary>
        /// Gets a dictionary of assembly names to their associated NoWarn codes from a raw string.
        /// </summary>
        /// <param name="noWarnAssembliesString">
        /// The raw NoWarn assemblies configuration string.
        /// The string follows the format: "AssemblyName1|Code1,Code2;AssemblyName2|Code3,Code4"
        /// where:
        /// - Each assembly entry is separated by semicolons (;).
        /// - Each assembly entry consists of an assembly name and NoWarn codes separated by a pipe character (|).
        /// - NoWarn codes are comma-separated.
        /// </param>
        /// <returns>A dictionary where the key is the assembly name and the value is a collection of NoWarn codes.</returns>
        public Dictionary<string, IEnumerable<string>> GetNoWarnByAssembly(string noWarnAssembliesString)
        {
            if (string.IsNullOrEmpty(noWarnAssembliesString))
            {
                return EmptyDictionary;
            }

            var entries = noWarnAssembliesString.Split(AssemblyEntriesSeparator, StringSplitOptions.RemoveEmptyEntries);

            if (entries.Length == 0)
            {
                return EmptyDictionary;
            }

            var result = new Dictionary<string, IEnumerable<string>>(entries.Length);
            foreach (var entry in entries)
            {
                var parts = entry.Split(EntryPartsSeparator, 2);
                if (parts.Length == 2)
                {
                    var assemblyName = parts[0];
                    var noWarnCodesString = parts[1];

                    if (string.IsNullOrEmpty(noWarnCodesString))
                    {
                        result[assemblyName] = Array.Empty<string>();
                    }
                    else
                    {
                        var noWarnCodes = noWarnCodesString.Split(NoWarnCodesSeparator, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(code => code.Trim())
                                                    .ToArray();

                        result[assemblyName] = noWarnCodes;
                    }
                }
            }

            return result;
        }
    }
}
