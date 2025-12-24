namespace ReferenceCop.Benchmarks
{
    using System.Collections.Generic;
    using System.Linq;
    using BenchmarkDotNet.Attributes;
    using Microsoft.CodeAnalysis;

    [MemoryDiagnoser]
    [RankColumn]
    public class AssemblyNameViolationDetectorBenchmarks
    {
        private AssemblyNameViolationDetector detector;
        private List<ReferenceEvaluationContext<AssemblyIdentity>> references;

        [Params(10, 50, 100)]
        public int RuleCount { get; set; }

        [Params(10, 100, 500)]
        public int ReferenceCount { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var config = this.CreateConfig(this.RuleCount);
            this.detector = new AssemblyNameViolationDetector(new PatternMatchComparer(), config);
            this.references = this.CreateReferences(this.ReferenceCount);
        }

        [Benchmark(Baseline = true)]
        public int Original()
        {
            return this.detector.GetViolationsFrom(this.references).Count();
        }

        [Benchmark]
        public int Experimental()
        {
            return this.detector.GetViolationsFromExperimental(this.references).Count();
        }

        private ReferenceCopConfig CreateConfig(int ruleCount)
        {
            var config = new ReferenceCopConfig();
            var rules = new List<ReferenceCopConfig.Rule>();

            // Create a mix of exact match and pattern rules (70% exact, 30% patterns)
            for (int i = 0; i < ruleCount; i++)
            {
                string pattern;
                if (i % 10 < 7)
                {
                    // Exact match
                    pattern = $"ExactMatch.Assembly{i}";
                }
                else
                {
                    // Pattern match
                    pattern = $"Pattern.Assembly{i}.*";
                }

                rules.Add(new ReferenceCopConfig.AssemblyName
                {
                    Pattern = pattern,
                    Severity = ReferenceCopConfig.Rule.ViolationSeverity.Warning,
                    Description = $"Reference to {pattern} is discouraged",
                });
            }

            config.Rules = rules;
            return config;
        }

        private List<ReferenceEvaluationContext<AssemblyIdentity>> CreateReferences(int count)
        {
            var references = new List<ReferenceEvaluationContext<AssemblyIdentity>>();

            for (int i = 0; i < count; i++)
            {
                string assemblyName;

                // Create a mix where some match rules and some don't
                if (i % 5 == 0)
                {
                    // Exact match (will match exact rules)
                    assemblyName = $"ExactMatch.Assembly{i % this.RuleCount}";
                }
                else if (i % 5 == 1)
                {
                    // Pattern match (will match pattern rules)
                    assemblyName = $"Pattern.Assembly{i % this.RuleCount}.SubAssembly";
                }
                else
                {
                    // No match
                    assemblyName = $"NoMatch.Assembly{i}";
                }

                references.Add(ReferenceEvaluationContextFactory.Create(new AssemblyIdentity(assemblyName)));
            }

            return references;
        }
    }
}
