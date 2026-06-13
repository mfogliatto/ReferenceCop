namespace ReferenceCop
{
    using System.Collections.Generic;

    /// <summary>
    /// Provider interface for accessing NoWarn assembly configurations.
    /// </summary>
    public interface INoWarnAssembliesProvider
    {
        /// <summary>
        /// Gets a dictionary of assembly names to their associated NoWarn codes from a raw string.
        /// </summary>
        /// <param name="noWarnAssembliesString">
        /// The raw NoWarn assemblies configuration string.
        /// The string follows the format: "AssemblyName1|Code1,Code2;AssemblyName2|Code3,Code4"
        /// where:
        /// - Each assembly entry is separated by semicolons (;)
        /// - Each assembly entry consists of an assembly name and NoWarn codes separated by a pipe character (|)
        /// - NoWarn codes are comma-separated
        /// </param>
        /// <returns>A dictionary where the key is the assembly name and the value is a set of NoWarn codes for O(1) lookups.</returns>
        Dictionary<string, HashSet<string>> GetNoWarnByAssembly(string noWarnAssembliesString);
    }
}
