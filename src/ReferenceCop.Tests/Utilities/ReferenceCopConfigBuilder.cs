namespace ReferenceCop.Tests
{
    using Microsoft.CodeAnalysis;

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
                Severity = DiagnosticSeverity.Error,
                Pattern = pattern
            });

            return this;
        }

        public ReferenceCopConfig Build()
        {
            return this.instance;
        }
    }
}
