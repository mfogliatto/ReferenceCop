namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System.Collections.Immutable;
    using System.Diagnostics;
        
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReferenceCopAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DiagnosticDescriptors.IllegalReferenceRule);

        private IViolationDetector<AssemblyIdentity> detector;

        public override void Initialize(AnalysisContext context)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            context.RegisterCompilationAction(compilationAnalysisContext =>
            {
                var configLoader = new XmlConfigurationLoader(compilationAnalysisContext);
                var config = configLoader.Load();
                this.detector = new AssemblyNameViolationDetector(new PatternMatchComparer(), config);
                this.AnalyzeCompilation(compilationAnalysisContext);
            });
        }

        private void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var compilation = context.Compilation;
            foreach (var illegalReference in this.detector.GetViolationsFrom(compilation.ReferencedAssemblyNames))
            {
                context.ReportDiagnostic(illegalReference);
            }
        }
    }
}
