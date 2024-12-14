namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    public class AssemblyTagViolationDetector : IViolationDetector<string>
    {
        private readonly ICollection<ReferenceCopConfig.AssemblyTag> rules = new List<ReferenceCopConfig.AssemblyTag>();

        private readonly string projectFilePath;
        private readonly IAssemblyTagProvider assemblyTagProvider;

        public AssemblyTagViolationDetector(ReferenceCopConfig config, string projectFilePath, IAssemblyTagProvider assemblyTagProvider)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
            this.assemblyTagProvider = assemblyTagProvider;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<string> references)
        {
            var fromAssemblyTag = assemblyTagProvider.GetAssemblyTag(projectFilePath);

            foreach (var rule in rules)
            {
                if (fromAssemblyTag == rule.FromAssemblyTag)
                {
                    foreach (var reference in references)
                    {
                        var toAssemblyTag = assemblyTagProvider.GetAssemblyTag(reference);

                        if (toAssemblyTag == rule.ToAssemblyTag)
                        {
                            yield return new Violation(rule, reference);
                        }
                    }
                }
            }
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
