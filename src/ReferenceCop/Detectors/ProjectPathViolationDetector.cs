namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectPathViolationDetector : IViolationDetector<string>
    {
        private readonly ICollection<ReferenceCopConfig.ProjectPath> rules = new List<ReferenceCopConfig.ProjectPath>();

        private readonly string projectFilePath;
        private readonly IProjectPathProvider projectPathProvider;
        private readonly ITraceWriter traceWriter;

        public ProjectPathViolationDetector(ReferenceCopConfig config, string projectFilePath, IProjectPathProvider projectPathProvider)
            : this(config, projectFilePath, projectPathProvider, NullTraceWriter.Instance)
        {
        }

        public ProjectPathViolationDetector(ReferenceCopConfig config, string projectFilePath, IProjectPathProvider projectPathProvider, ITraceWriter traceWriter)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
            this.projectPathProvider = projectPathProvider;
            this.traceWriter = traceWriter ?? NullTraceWriter.Instance;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<ReferenceEvaluationContext<string>> references)
        {
            var fromProjectPath = this.projectPathProvider.GetRelativePath(this.projectFilePath);

            if (this.traceWriter.IsEnabled)
            {
                this.traceWriter.Write($"[ProjectPathViolationDetector] Evaluating project '{this.projectFilePath}' with relative path '{fromProjectPath}'");
                this.traceWriter.Write($"[ProjectPathViolationDetector] Loaded {this.rules.Count} rule(s)");
            }

            foreach (var rule in this.rules)
            {
                if (fromProjectPath.StartsWith(rule.FromPath))
                {
                    if (this.traceWriter.IsEnabled)
                    {
                        this.traceWriter.Write($"[ProjectPathViolationDetector] Rule '{rule.Name}': FromPath '{rule.FromPath}' matches project path '{fromProjectPath}'");
                    }

                    foreach (var referenceContext in references)
                    {
                        var toProjectPath = this.projectPathProvider.GetRelativePath(referenceContext.Reference);

                        if (toProjectPath.StartsWith(rule.ToPath))
                        {
                            // Check if this warning should be suppressed
                            if (referenceContext.IsWarningSuppressed)
                            {
                                if (this.traceWriter.IsEnabled)
                                {
                                    this.traceWriter.Write($"[ProjectPathViolationDetector] Rule '{rule.Name}': violation suppressed for reference '{referenceContext.Reference}'");
                                }

                                continue;
                            }

                            if (this.traceWriter.IsEnabled)
                            {
                                this.traceWriter.Write($"[ProjectPathViolationDetector] Rule '{rule.Name}': VIOLATION for reference '{referenceContext.Reference}' (ToPath='{toProjectPath}')");
                            }

                            yield return new Violation(rule, referenceContext.Reference);
                        }
                        else if (this.traceWriter.IsEnabled)
                        {
                            this.traceWriter.Write($"[ProjectPathViolationDetector] Rule '{rule.Name}': reference path '{toProjectPath}' does not start with '{rule.ToPath}', no match");
                        }
                    }
                }
                else if (this.traceWriter.IsEnabled)
                {
                    this.traceWriter.Write($"[ProjectPathViolationDetector] Rule '{rule.Name}': FromPath '{rule.FromPath}' does not match project path '{fromProjectPath}', skipping");
                }
            }
        }

        public IEnumerable<Violation> GetViolationsFromExperimental(IEnumerable<ReferenceEvaluationContext<string>> references)
        {
            return this.GetViolationsFrom(references);
        }

        private void LoadRulesFrom(ReferenceCopConfig config)
        {
            var projectPathRules = config.Rules.OfType<ReferenceCopConfig.ProjectPath>();
            foreach (var rule in projectPathRules)
            {
                this.rules.Add(rule);
            }
        }
    }
}
