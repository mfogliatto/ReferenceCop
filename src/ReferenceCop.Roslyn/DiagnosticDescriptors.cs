namespace ReferenceCop.Roslyn
{
    using Microsoft.CodeAnalysis;

    internal class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor GeneralError = new DiagnosticDescriptor(
            "RC0000",
            "General Error",
            "An error occurred while executing the analyzer: {0}",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

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

        public static readonly DiagnosticDescriptor DebugMessage = new DiagnosticDescriptor(
            "RC9999",
            "Debug Message",
            "[DEBUG]: {0}",
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        private const string Category = "ReferenceCop";
    }
}
