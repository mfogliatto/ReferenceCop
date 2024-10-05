namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class AssemblyNameViolationDetector : IViolationDetector
    {
        private readonly Dictionary<string, ReferenceCopConfig.Rule> rules;
        private readonly IEqualityComparer<string> referenceNameComparer;

        private ReferenceCopConfig config;

        public AssemblyNameViolationDetector(IEqualityComparer<string> referenceNameComparer, ReferenceCopConfig config)
        {
            this.rules = new Dictionary<string, ReferenceCopConfig.Rule>(referenceNameComparer);
            this.LoadRulesFrom(config);
            this.referenceNameComparer = referenceNameComparer;
        }

        public IEnumerable<Diagnostic> GetViolationsFrom(IEnumerable<AssemblyIdentity> references)
        {
            foreach (var reference in references)
            {
                if (this.rules.ContainsKey(reference.Name))
                {
                    yield return DiagnosticFactory.CreateDiagnosticFor(this.rules[reference.Name]);
                }
            }
        }

        private void LoadRulesFrom(ReferenceCopConfig config)
        {
            var assemblyNameRules = config.Rules.OfType<ReferenceCopConfig.AssemblyName>();
            foreach (var rule in assemblyNameRules)
            {
                try
                {
                    this.rules.Add(rule.Pattern, rule);
                    break;
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException($"Duplicate rule pattern '{rule.Pattern}' found in the configuration file.");
                }
            }
        }
    }
}
