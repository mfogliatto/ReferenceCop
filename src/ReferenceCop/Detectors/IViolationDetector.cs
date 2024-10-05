namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;

    internal interface IViolationDetector
    {
        IEnumerable<Diagnostic> GetViolationsFrom(IEnumerable<AssemblyIdentity> references);
    }
}