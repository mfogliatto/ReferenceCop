namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;

    internal interface IIllegalReferenceDetector
    {
        void Initialize(CompilationAnalysisContext compilationAnalysisContext);
        IEnumerable<Diagnostic> GetIllegalReferencesFrom(IEnumerable<AssemblyIdentity> references);
    }
}
