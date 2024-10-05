using Microsoft.CodeAnalysis;

namespace ReferenceCop
{
    internal static class DiagnosticFactory
    {
        public static Diagnostic CreateDiagnosticFor(ReferenceCopConfig.Rule rule)
        {
            switch (rule.Severity)
            {
                case DiagnosticSeverity.Error:
                    return Diagnostic.Create(DiagnosticDescriptors.IllegalReferenceRule, Location.None, rule.Name);
                case DiagnosticSeverity.Warning:
                    return Diagnostic.Create(DiagnosticDescriptors.DiscouragedReferenceRule, Location.None, rule.Name);
                default:
                    return null;
            }
        }
    }
}
