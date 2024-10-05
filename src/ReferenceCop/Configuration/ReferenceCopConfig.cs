namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;

    public class ReferenceCopConfig
    {
        public List<Rule> Rules { get; set; }

        public class Rule
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Pattern { get; set; }
            public DiagnosticSeverity Severity { get; set; }
        }
    }
}
