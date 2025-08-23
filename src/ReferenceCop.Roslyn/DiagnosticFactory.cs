namespace ReferenceCop.Roslyn
{
    using System;
    using Microsoft.CodeAnalysis;

    internal static class DiagnosticFactory
    {
        public static Diagnostic CreateFor(Violation violation)
        {
            switch (violation.Rule.Severity)
            {
                case ReferenceCopConfig.Rule.ViolationSeverity.Error:
                    return Diagnostic.Create(
                        DiagnosticDescriptors.IllegalReferenceRule,
                        Location.None,
                        violation.ReferenceName,
                        violation.Rule.Name,
                        violation.Rule.Description);
                case ReferenceCopConfig.Rule.ViolationSeverity.Warning:
                    return Diagnostic.Create(
                        DiagnosticDescriptors.DiscouragedReferenceRule,
                        Location.None,
                        violation.ReferenceName,
                        violation.Rule.Name,
                        violation.Rule.Description);
                default:
                    return null;
            }
        }

        public static Diagnostic CreateFor(Exception ex)
        {
            return Diagnostic.Create(
                DiagnosticDescriptors.GeneralError,
                Location.None,
                ex.Message);
        }
    }
}
