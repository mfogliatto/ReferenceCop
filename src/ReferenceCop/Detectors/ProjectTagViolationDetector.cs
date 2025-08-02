namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectTagViolationDetector : IViolationDetector<string>
    {
        private readonly ICollection<ReferenceCopConfig.ProjectTag> rules = new List<ReferenceCopConfig.ProjectTag>();

        private readonly string projectFilePath;
        private readonly IProjectTagProvider projectTagProvider;

        public ProjectTagViolationDetector(ReferenceCopConfig config, string projectFilePath, IProjectTagProvider projectTagProvider)
        {
            this.LoadRulesFrom(config);
            this.projectFilePath = projectFilePath;
            this.projectTagProvider = projectTagProvider;
        }

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<ReferenceEvaluationContext<string>> references)
        {
            var fromProjectTag = this.projectTagProvider.GetProjectTag(this.projectFilePath);

            foreach (var rule in this.rules)
            {
                if (fromProjectTag == rule.FromProjectTag)
                {
                    foreach (var referenceContext in references)
                    {
                        var toProjectTag = this.projectTagProvider.GetProjectTag(referenceContext.Reference);

                        if (toProjectTag == rule.ToProjectTag)
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
            var projectTagRules = config.Rules.OfType<ReferenceCopConfig.ProjectTag>();
            foreach (var rule in projectTagRules)
            {
                this.rules.Add(rule);
            }
        }
    }
}
