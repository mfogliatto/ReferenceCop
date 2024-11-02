namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticFactory
    {
        public static Diagnostic CreateDiagnosticFor(ReferenceCopConfig.Rule rule, string referenceName)
        {
            switch (rule.Severity)
            {
                case ReferenceCopConfig.Rule.ViolationSeverity.Error:
                    return Diagnostic.Create(DiagnosticDescriptors.IllegalReferenceRule, Location.None, referenceName, rule.Name);
                case ReferenceCopConfig.Rule.ViolationSeverity.Warning:
                    return Diagnostic.Create(DiagnosticDescriptors.DiscouragedReferenceRule, Location.None, referenceName, rule.Name);
                default:
                    return null;
            }
        }
    }
}
