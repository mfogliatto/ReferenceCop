namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;

    public class AssemblyNameViolationDetector : IViolationDetector<AssemblyIdentity>
    {
        private readonly Dictionary<string, ReferenceCopConfig.Rule> rules;
        private readonly Dictionary<string, ReferenceCopConfig.Rule> exactMatchRules;
        private readonly List<KeyValuePair<string, ReferenceCopConfig.Rule>> patternRules;
        private readonly IEqualityComparer<string> referenceNameComparer;

        public AssemblyNameViolationDetector(IEqualityComparer<string> referenceNameComparer, ReferenceCopConfig config)
        {
            this.rules = new Dictionary<string, ReferenceCopConfig.Rule>(referenceNameComparer);
            this.referenceNameComparer = referenceNameComparer;

            // Separate exact matches from patterns for performance optimization.
            this.exactMatchRules = new Dictionary<string, ReferenceCopConfig.Rule>(StringComparer.InvariantCulture);
            this.patternRules = new List<KeyValuePair<string, ReferenceCopConfig.Rule>>();

            this.LoadRulesFrom(config);
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<ReferenceEvaluationContext<AssemblyIdentity>> references)
        {
            foreach (var rule in this.rules)
            {
                foreach (var referenceContext in references)
                {
                    var reference = referenceContext.Reference;
                    if (string.IsNullOrEmpty(reference?.Name))
                    {
                        throw new InvalidOperationException("Reference name cannot be null or empty.");
                    }

                    if (this.referenceNameComparer.Equals(rule.Key, reference.Name))
                    {
                        // Check if this warning should be suppressed
                        if (referenceContext.IsWarningSuppressed)
                        {
                            continue;
                        }

                        yield return new Violation(rule.Value, reference.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Optimized O(n) version of GetViolationsFrom for PatternMatchComparer.
        /// Separates exact match rules from wildcard patterns for improved performance.
        /// Exact matches use O(1) dictionary lookup, while patterns use O(n*p) iteration where p is pattern count.
        /// Total complexity: O(n) for exact matches only, O(n*p) when patterns exist (much better than O(n*m) where m = total rules).
        /// </summary>
        /// <param name="references">The references to evaluate.</param>
        /// <returns>Violations found in the references.</returns>
        public IEnumerable<Violation> GetViolationsFromExperimental(IEnumerable<ReferenceEvaluationContext<AssemblyIdentity>> references)
        {
            // O(n) iteration through references, with O(1) lookup for exact matches
            foreach (var referenceContext in references)
            {
                var reference = referenceContext.Reference;
                if (string.IsNullOrEmpty(reference?.Name))
                {
                    throw new InvalidOperationException("Reference name cannot be null or empty.");
                }

                // Skip if warning is suppressed
                if (referenceContext.IsWarningSuppressed)
                {
                    continue;
                }

                // Check exact match rules with O(1) lookup
                if (this.exactMatchRules.TryGetValue(reference.Name, out var exactRule))
                {
                    yield return new Violation(exactRule, reference.Name);
                }

                // Check pattern rules - only iterate through patterns (typically much smaller than total rules)
                foreach (var patternRule in this.patternRules)
                {
                    if (this.referenceNameComparer.Equals(patternRule.Key, reference.Name))
                    {
                        yield return new Violation(patternRule.Value, reference.Name);
                    }
                }
            }
        }

        private static bool IsExactMatch(string pattern)
        {
            // A pattern is exact if it doesn't contain wildcards and isn't the default "*" pattern
            return pattern != "*" && !pattern.Contains('*');
        }

        private void LoadRulesFrom(ReferenceCopConfig config)
        {
            var assemblyNameRules = config.Rules.OfType<ReferenceCopConfig.AssemblyName>();
            foreach (var rule in assemblyNameRules)
            {
                // Load into original dictionary for backward compatibility
                try
                {
                    this.rules.Add(rule.Pattern, rule);
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException($"Duplicate rule pattern '{rule.Pattern}' found in the configuration file.");
                }

                // Also load into optimized structures - separate exact matches from patterns
                if (IsExactMatch(rule.Pattern))
                {
                    this.exactMatchRules.Add(rule.Pattern, rule);
                }
                else
                {
                    this.patternRules.Add(new KeyValuePair<string, ReferenceCopConfig.Rule>(rule.Pattern, rule));
                }
            }
        }
    }
}
