namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;

    internal class DiagnosticDescriptors
    {
        private const string Category = "ReferenceCop";

        public static readonly DiagnosticDescriptor IllegalReferenceRule = new DiagnosticDescriptor(
            Violation.ViolationSeverityErrorCode,
            "Illegal references",
            ViolationMessageTemplates.IllegalReference,
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor DiscouragedReferenceRule = new DiagnosticDescriptor(
            Violation.ViolationSeverityWarningCode,
            "Discouraged references",
            ViolationMessageTemplates.DiscouragedReference,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
    }
}
