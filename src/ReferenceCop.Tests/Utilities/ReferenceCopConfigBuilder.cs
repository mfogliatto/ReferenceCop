namespace ReferenceCop.Tests
{
    internal class ReferenceCopConfigBuilder
    {
        private readonly ReferenceCopConfig instance;

        public ReferenceCopConfigBuilder()
        {
            this.instance = new ReferenceCopConfig();
        }

        public ReferenceCopConfigBuilder WithAssemblyNameRule(string pattern)
        {
            this.instance.Rules.Add(new ReferenceCopConfig.AssemblyName
            {
                Name = "DoNotUse-" + pattern,
                Description = $"Use of {pattern} is forbidden. Please use Y instead",
                Severity = ReferenceCopConfig.Rule.ViolationSeverity.Error,
                Pattern = pattern,
            });

            return this;
        }

        public ReferenceCopConfig Build()
        {
            return this.instance;
        }
    }
}
