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

        public IEnumerable<Violation> GetViolationsFrom(IEnumerable<string> references)
        {
            var fromProjectPath = projectPathProvider.GetRelativePath(projectFilePath);

            foreach (var rule in rules)
            {
                if (fromProjectPath.StartsWith(rule.FromPath))
                {
                    foreach (var reference in references)
                    {
                        var toProjectPath = projectPathProvider.GetRelativePath(reference);

                        if (toProjectPath.StartsWith(rule.ToPath))
                        {
                            yield return new Violation(rule, reference);
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
