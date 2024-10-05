namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    internal class ConfigurableILlegalReferenceDetector : IIllegalReferenceDetector
    {
        private readonly Dictionary<string, ReferenceCopConfig.Rule> rules;
        private readonly IEqualityComparer<string> referenceNameComparer;

        private ReferenceCopConfig config;

        public ConfigurableILlegalReferenceDetector(IEqualityComparer<string> referenceNameComparer, ReferenceCopConfig config)
        {
            this.rules = new Dictionary<string, ReferenceCopConfig.Rule>();
            this.LoadRulesFrom(config);
            this.referenceNameComparer = referenceNameComparer;
        }

        public IEnumerable<Diagnostic> GetIllegalReferencesFrom(IEnumerable<AssemblyIdentity> references)
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
            foreach (var rule in config.Rules)
            {
                try
                {
                    this.rules.Add(rule.Pattern, rule);
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException($"Duplicate rule pattern '{rule.Pattern}' found in the configuration file.");
                }
            }
        }
    }
}
