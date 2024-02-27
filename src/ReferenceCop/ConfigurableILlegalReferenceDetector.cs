using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;

namespace ReferenceCop
{
    internal class ConfigurableILlegalReferenceDetector : IIllegalReferenceDetector
    {
        private readonly string configFilePath;
        private readonly HashSet<string> illegalAssemblyNames;

        public ConfigurableILlegalReferenceDetector(string configFilePath)
        {
            this.configFilePath = configFilePath;
            this.illegalAssemblyNames = new HashSet<string>();
        }

        public void Initialize(CompilationAnalysisContext compilationAnalysisContext)
        {
            ImmutableArray<AdditionalText> additionalFiles = compilationAnalysisContext.Options.AdditionalFiles;
            AdditionalText illegalReferencesFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals(this.configFilePath));

            if (illegalReferencesFile == null)
            {
                throw new InvalidOperationException("Configuration file containing illegal references was not found.");
            }

            this.LoadIllegalAssemblyNames(illegalReferencesFile, compilationAnalysisContext.CancellationToken);
        }

        public IEnumerable<Diagnostic> GetIllegalReferencesFrom(IEnumerable<AssemblyIdentity> references)
        {
            foreach (var reference in references)
            {
                if (this.illegalAssemblyNames.Contains(reference.Name))
                {
                    // Report a diagnostic for the illegal reference
                    yield return DiagnosticFactory.CreateIllegalReferenceDiagnosticFor(reference.Name);
                }
            }
        }

        private void LoadIllegalAssemblyNames(AdditionalText illegalReferencesFile, CancellationToken cancellationToken)
        {
            try
            {
                SourceText sourceText = illegalReferencesFile.GetText(cancellationToken);
                foreach (TextLine line in sourceText.Lines)
                {
                    this.illegalAssemblyNames.Add(line.ToString());
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
