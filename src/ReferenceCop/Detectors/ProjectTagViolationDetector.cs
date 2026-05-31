namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectTagViolationDetector : IViolationDetector<string>
    {
        private readonly ICollection<ReferenceCopConfig.ProjectTag> rules = new List<ReferenceCopConfig.ProjectTag>();

        private readonly string projectFilePath;
        private readonly IProjectTagProvider projectTagProvider;
        private readonly ITraceWriter traceWriter;

        public ProjectTagViolationDetector(ReferenceCopConfig config, string projectFilePath, IProjectTagProvider projectTagProvider)
            : this(config, projectFilePath, projectTagProvider, NullTraceWriter.Instance)
        {
        }

        public ProjectTagViolationDetector(ReferenceCopConfig config, string projectFilePath, IProjectTagProvider projectTagProvider, ITraceWriter traceWriter)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
            this.projectTagProvider = projectTagProvider;
            this.traceWriter = traceWriter ?? NullTraceWriter.Instance;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<ReferenceEvaluationContext<string>> references)
        {
            var fromProjectTag = this.projectTagProvider.GetProjectTag(this.projectFilePath);

            if (this.traceWriter.IsEnabled)
            {
                this.traceWriter.Write($"[ProjectTagViolationDetector] Evaluating project '{this.projectFilePath}' with tag '{fromProjectTag}'");
                this.traceWriter.Write($"[ProjectTagViolationDetector] Loaded {this.rules.Count} rule(s)");
            }

            foreach (var rule in this.rules)
            {
                if (fromProjectTag == rule.FromProjectTag)
                {
                    if (this.traceWriter.IsEnabled)
                    {
                        this.traceWriter.Write($"[ProjectTagViolationDetector] Rule '{rule.Name}' matched FromProjectTag '{rule.FromProjectTag}'");
                    }

                    foreach (var referenceContext in references)
                    {
                        var toProjectTag = this.projectTagProvider.GetProjectTag(referenceContext.Reference);

                        if (toProjectTag == rule.ToProjectTag)
                        {
                            // Check if this warning should be suppressed
                            if (referenceContext.IsWarningSuppressed)
                            {
                                if (this.traceWriter.IsEnabled)
                                {
                                    this.traceWriter.Write($"[ProjectTagViolationDetector] Rule '{rule.Name}': violation suppressed for reference '{referenceContext.Reference}'");
                                }

                                continue;
                            }

                            if (this.traceWriter.IsEnabled)
                            {
                                this.traceWriter.Write($"[ProjectTagViolationDetector] Rule '{rule.Name}': VIOLATION for reference '{referenceContext.Reference}' (ToProjectTag='{toProjectTag}')");
                            }

                            yield return new Violation(rule, referenceContext.Reference);
                        }
                        else if (this.traceWriter.IsEnabled)
                        {
                            this.traceWriter.Write($"[ProjectTagViolationDetector] Rule '{rule.Name}': reference '{referenceContext.Reference}' has tag '{toProjectTag}', no match with '{rule.ToProjectTag}'");
                        }
                    }
                }
                else if (this.traceWriter.IsEnabled)
                {
                    this.traceWriter.Write($"[ProjectTagViolationDetector] Rule '{rule.Name}': FromProjectTag '{rule.FromProjectTag}' does not match project tag '{fromProjectTag}', skipping");
                }
            }
        }

        public IEnumerable<Violation> GetViolationsFromExperimental(IEnumerable<ReferenceEvaluationContext<string>> references)
        {
            return this.GetViolationsFrom(references);
        }

        private void LoadRulesFrom(ReferenceCopConfig config)
        {
            var projectTagRules = config.Rules.OfType<ReferenceCopConfig.ProjectTag>();
            foreach (var rule in projectTagRules)
            {
                this.rules.Add(rule);
            }
        }
    }
}
