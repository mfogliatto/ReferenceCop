namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class StaticIllegalReferenceDetector : IViolationDetector
    {
        private readonly string[] illegalReferences = new[] { "Newtonsoft.Json", "LibraryB" };
        
        public IEnumerable<Diagnostic> GetViolationsFrom(IEnumerable<AssemblyIdentity> references)
        {
            foreach (var reference in references)
            {
                if (this.illegalReferences.Contains(reference.Name))
                {
                    yield return DiagnosticFactory.CreateIllegalReferenceDiagnosticFor(reference.Name);
                }
            }
        }
    }
}
