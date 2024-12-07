namespace ReferenceCop
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReferenceCopAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            DiagnosticDescriptors.GeneralError,
            DiagnosticDescriptors.IllegalReferenceRule,
            DiagnosticDescriptors.DiscouragedReferenceRule);

        private IViolationDetector<AssemblyIdentity> detector;

        public override void Initialize(AnalysisContext context)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            context.RegisterCompilationAction(compilationAnalysisContext =>
            {
                try
                {
                    var configLoader = new XmlConfigurationLoader(compilationAnalysisContext);
                    var config = configLoader.Load();
                    this.detector = new AssemblyNameViolationDetector(new PatternMatchComparer(), config);
                    this.AnalyzeCompilation(compilationAnalysisContext);
                }
                catch (Exception ex)
                {
                    compilationAnalysisContext.ReportDiagnostic(DiagnosticFactory.CreateFor(ex));
                }
            });
        }

        private void AnalyzeCompilation(CompilationAnalysisContext compilationAnalysisContext)
        {
            var compilation = compilationAnalysisContext.Compilation;
            foreach (var violation in this.detector.GetViolationsFrom(compilation.ReferencedAssemblyNames))
            {
                compilationAnalysisContext.ReportDiagnostic(DiagnosticFactory.CreateFor(violation));
            }
        }
    }
}
