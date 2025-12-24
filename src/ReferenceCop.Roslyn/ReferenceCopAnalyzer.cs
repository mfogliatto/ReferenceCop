namespace ReferenceCop.Roslyn
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReferenceCopAnalyzer : DiagnosticAnalyzer
    {
        private const string LaunchDebuggerKey = "build_property.LaunchDebugger";
        private const string RoslynDebuggerTriggerValue = "Roslyn";
        private const string NoWarnAssembliesKey = "build_property.ReferenceCop_NoWarnAssemblies";

        private readonly INoWarnAssembliesProvider noWarnAssembliesProvider;
        private IViolationDetector<AssemblyIdentity> assemblyNameViolationDetector;
        private ReferenceCopConfig config;

        public ReferenceCopAnalyzer()
        {
            this.noWarnAssembliesProvider = new NoWarnAssembliesProvider();
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.GeneralError,
        DiagnosticDescriptors.IllegalReferenceRule,
        DiagnosticDescriptors.DiscouragedReferenceRule,
        DiagnosticDescriptors.DebugMessage);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationAction(compilationAnalysisContext =>
            {
                LaunchDebuggerIfRequested(compilationAnalysisContext);

                try
                {
                    var configLoader = new XmlConfigurationLoader(compilationAnalysisContext);
                    this.config = configLoader.Load();
                    this.assemblyNameViolationDetector = new AssemblyNameViolationDetector(new PatternMatchComparer(), this.config);

                    if (this.config.EnableDebugMessages && this.config.UseExperimentalDetectors)
                    {
                        compilationAnalysisContext.ReportDiagnostic(
                            DiagnosticFactory.CreateDebugMessage("Using experimental detectors"));
                    }

                    this.AnalyzeCompilation(compilationAnalysisContext);
                }
                catch (Exception ex)
                {
                    compilationAnalysisContext.ReportDiagnostic(DiagnosticFactory.CreateFor(ex));
                }
            });
        }

        private static void LaunchDebuggerIfRequested(CompilationAnalysisContext compilationAnalysisContext)
        {
            var isConfigPresent = compilationAnalysisContext.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(LaunchDebuggerKey, out var launchDebuggerValue);
            bool launchDebuggerRequested = isConfigPresent && launchDebuggerValue.Contains(RoslynDebuggerTriggerValue);
            if (!Debugger.IsAttached && launchDebuggerRequested)
            {
                Debugger.Launch();
            }
        }

        private void AnalyzeCompilation(CompilationAnalysisContext compilationAnalysisContext)
        {
            var compilation = compilationAnalysisContext.Compilation;
            compilationAnalysisContext.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(NoWarnAssembliesKey, out string noWarnAssemblies);
            var noWarnByAssembly = this.noWarnAssembliesProvider.GetNoWarnByAssembly(noWarnAssemblies);

            var evaluationContexts = compilation.ReferencedAssemblyNames
                .Select(assemblyRef =>
                {
                    var assemblyName = assemblyRef.Name;
                    var noWarnCodes = noWarnByAssembly.TryGetValue(assemblyName, out var codes) ? codes : null;
                    return ReferenceEvaluationContextFactory.Create(assemblyRef, noWarnCodes);
                })
                .ToList();

            var violations = this.config.UseExperimentalDetectors
                ? this.assemblyNameViolationDetector.GetViolationsFromExperimental(evaluationContexts)
                : this.assemblyNameViolationDetector.GetViolationsFrom(evaluationContexts);

            foreach (var violation in violations)
            {
                compilationAnalysisContext.ReportDiagnostic(DiagnosticFactory.CreateFor(violation));
            }
        }
    }
}
