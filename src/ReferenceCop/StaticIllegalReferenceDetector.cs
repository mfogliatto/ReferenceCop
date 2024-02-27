using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReferenceCop
{
    internal class StaticIllegalReferenceDetector : IIllegalReferenceDetector
    {
        private readonly string[] illegalReferences = new[] { "Newtonsoft.Json", "LibraryB" };
        
        public void Initialize(CompilationAnalysisContext compilationAnalysisContext)
        {
        }
        
        public IEnumerable<Diagnostic> GetIllegalReferencesFrom(IEnumerable<AssemblyIdentity> references)
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
