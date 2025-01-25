namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class AssemblyNameViolationDetector : IViolationDetector<AssemblyIdentity>
    {
        private readonly Dictionary<string, ReferenceCopConfig.Rule> rules;
        private readonly IEqualityComparer<string> referenceNameComparer;

        public AssemblyNameViolationDetector(IEqualityComparer<string> referenceNameComparer, ReferenceCopConfig config)
        {
            this.rules = new Dictionary<string, ReferenceCopConfig.Rule>(referenceNameComparer);
            this.LoadRulesFrom(config);
            this.referenceNameComparer = referenceNameComparer;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<AssemblyIdentity> references)
        {
            foreach (var rule in this.rules)
            {
                foreach (var reference in references)
                {
                    if (string.IsNullOrEmpty(reference?.Name))
                    {
                        throw new InvalidOperationException("Reference name cannot be null or empty.");
                    }

                    if (this.referenceNameComparer.Equals(rule.Key, reference.Name))
                    {
                        yield return new Violation(rule.Value, reference.Name);
                    }
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
