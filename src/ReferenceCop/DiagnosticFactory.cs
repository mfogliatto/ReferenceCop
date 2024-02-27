using Microsoft.CodeAnalysis;

namespace ReferenceCop
{
    internal static class DiagnosticFactory
    {
        public static Diagnostic CreateIllegalReferenceDiagnosticFor(string referenceName)
        {
            return Diagnostic.Create(DiagnosticDescriptors.IllegalReferenceRule, Location.None, referenceName);
        }
    }
}
