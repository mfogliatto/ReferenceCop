namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;

    internal class DiagnosticDescriptors
    {
        private const string Category = "ReferenceCop";

        public static readonly DiagnosticDescriptor IllegalReferenceRule = new DiagnosticDescriptor(
            "RC0001",
            "Illegal references",
            "Illegal reference '{0}' must be removed",
            Category,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
