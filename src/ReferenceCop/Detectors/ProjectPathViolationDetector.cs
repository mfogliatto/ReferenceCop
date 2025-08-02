namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectPathViolationDetector : IViolationDetector<string>
    {
        private readonly ICollection<ReferenceCopConfig.ProjectPath> rules = new List<ReferenceCopConfig.ProjectPath>();

        private readonly string projectFilePath;
        private readonly IProjectPathProvider projectPathProvider;

        public ProjectPathViolationDetector(ReferenceCopConfig config, string projectFilePath, IProjectPathProvider projectPathProvider)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
            this.projectPathProvider = projectPathProvider;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<ReferenceEvaluationContext<string>> references)
        {
            var fromProjectPath = this.projectPathProvider.GetRelativePath(this.projectFilePath);

            foreach (var rule in this.rules)
            {
                if (fromProjectPath.StartsWith(rule.FromPath))
                {
                    foreach (var referenceContext in references)
                    {
                        var toProjectPath = this.projectPathProvider.GetRelativePath(referenceContext.Reference);

                        if (toProjectPath.StartsWith(rule.ToPath))
                        {
                            // Check if this warning should be suppressed
                            if (referenceContext.IsWarningSuppressed)
                            {
                                continue;
                            }

                            yield return new Violation(rule, referenceContext.Reference);
                        }
                    }
                }
            }
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
