namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public class AssemblyTagViolationDetector : IViolationDetector<string>
    {
        internal const string PropertyGroupNode = "PropertyGroup";
        internal const string AssemblyTagNode = "AssemblyTag";
        internal const string UnknownAssemblyTag = "Unknown";

        private readonly ICollection<ReferenceCopConfig.AssemblyTag> rules = new List<ReferenceCopConfig.AssemblyTag>();
        private readonly string projectFilePath;

        public AssemblyTagViolationDetector(ReferenceCopConfig config, string projectFilePath)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<string> references)
        {
            var fromAssemblyTag = GetAssemblyTag(projectFilePath);

            foreach (var rule in rules)
            {
                if (fromAssemblyTag == rule.FromAssemblyTag)
                {
                    foreach (var reference in references)
                    {
                        var toAssemblyTag = GetAssemblyTag(reference);

                        if (toAssemblyTag == rule.ToAssemblyTag)
                        {
                            yield return new Violation(rule, reference);
                        }
                    }
                }
            }
        }

        private string GetAssemblyTag(string projectFilePath)
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

        private void LoadRulesFrom(ReferenceCopConfig config)
        {
            var assemblyNameRules = config.Rules.OfType<ReferenceCopConfig.AssemblyTag>();
            foreach (var rule in assemblyNameRules)
            {
                this.rules.Add(rule);
            }
        }
    }
}
