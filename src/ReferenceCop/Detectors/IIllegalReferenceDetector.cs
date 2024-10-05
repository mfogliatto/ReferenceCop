namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Generic;

    internal interface IIllegalReferenceDetector
    {
        IEnumerable<Diagnostic> GetIllegalReferencesFrom(IEnumerable<AssemblyIdentity> references);
    }
}
