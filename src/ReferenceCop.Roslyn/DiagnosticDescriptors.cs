namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;

    internal class DiagnosticDescriptors
    {
        private const string Category = "ReferenceCop";

        public static readonly DiagnosticDescriptor IllegalReferenceRule = new DiagnosticDescriptor(
            "RC0001",
            "Illegal references",
            "Illegal reference '{0}' must be removed, as its usage is forbidden by rule '{1}'",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor DiscouragedReferenceRule = new DiagnosticDescriptor(
            "RC0002",
            "Discouraged references",
            "Consider removing reference '{0}' as its usage is discouraged by rule '{1}'",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}
