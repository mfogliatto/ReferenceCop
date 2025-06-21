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

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<string> references)
        {
            var fromProjectTag = this.projectTagProvider.GetProjectTag(this.projectFilePath);

            foreach (var rule in this.rules)
            {
                if (fromProjectTag == rule.FromProjectTag)
                {
                    foreach (var reference in references)
                    {
                        var toProjectTag = this.projectTagProvider.GetProjectTag(reference);

                        if (toProjectTag == rule.ToProjectTag)
                        {
                            yield return new Violation(rule, reference);
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
