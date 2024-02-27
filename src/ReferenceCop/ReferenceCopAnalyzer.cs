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

        private const string ReferenceCopConfigPath = "ReferenceCop.config";
        private readonly IIllegalReferenceDetector detector;

        public ReferenceCopAnalyzer()
        {
            this.detector = new ConfigurableILlegalReferenceDetector(ReferenceCopConfigPath);
        }

        public override void Initialize(AnalysisContext context)
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            context.RegisterCompilationAction(compilationAnalysisContext =>
            {
                this.detector.Initialize(compilationAnalysisContext);
                AnalyzeCompilation(compilationAnalysisContext);
            });
        }

        private void AnalyzeCompilation(CompilationAnalysisContext context)
        {
            var compilation = context.Compilation;
            foreach (var illegalReference in this.detector.GetIllegalReferencesFrom(compilation.ReferencedAssemblyNames))
            {
                context.ReportDiagnostic(illegalReference);
            }
        }
    }
}
