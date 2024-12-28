namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    public class AssemblyPathViolationDetector : IViolationDetector<string>
    {
        private readonly ICollection<ReferenceCopConfig.AssemblyPath> rules = new List<ReferenceCopConfig.AssemblyPath>();

        private readonly string projectFilePath;
        private readonly IAssemblyPathProvider assemblyPathProvider;

        public AssemblyPathViolationDetector(ReferenceCopConfig config, string projectFilePath, IAssemblyPathProvider assemblyPathProvider)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
            this.assemblyPathProvider = assemblyPathProvider;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<string> references)
        {
            var fromAssemblyPath = assemblyPathProvider.GetAssemblyPath(projectFilePath);

            foreach (var rule in rules)
            {
                if (fromAssemblyPath.StartsWith(rule.FromPath))
                {
                    foreach (var reference in references)
                    {
                        var toAssemblyPath = assemblyPathProvider.GetAssemblyPath(reference);

                        if (toAssemblyPath.StartsWith(rule.ToPath))
                        {
                            yield return new Violation(rule, reference);
                        }
                    }
                }
            }
        }

        private void LoadRulesFrom(ReferenceCopConfig config)
        {
            var assemblyPathRules = config.Rules.OfType<ReferenceCopConfig.AssemblyPath>();
            foreach (var rule in assemblyPathRules)
            {
                this.rules.Add(rule);
            }
        }
    }
}
