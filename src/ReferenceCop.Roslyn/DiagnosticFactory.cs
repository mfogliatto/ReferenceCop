namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticFactory
    {
        public static Diagnostic CreateDiagnosticFor(Violation violation)
        {
            switch (violation.Rule.Severity)
            {
                case ReferenceCopConfig.Rule.ViolationSeverity.Error:
                    return Diagnostic.Create(DiagnosticDescriptors.IllegalReferenceRule, Location.None, violation.ReferenceName, violation.Rule.Name);
                case ReferenceCopConfig.Rule.ViolationSeverity.Warning:
                    return Diagnostic.Create(DiagnosticDescriptors.DiscouragedReferenceRule, Location.None, violation.ReferenceName, violation.Rule.Name);
                default:
                    return null;
            }
        }
    }
}
