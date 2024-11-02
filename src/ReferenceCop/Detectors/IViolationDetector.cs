namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using System.Collections.Generic;

    internal interface IViolationDetector<TAssemblyIdentity>
    {
        IEnumerable<Diagnostic> GetViolationsFrom(IEnumerable<TAssemblyIdentity> references);
    }
}