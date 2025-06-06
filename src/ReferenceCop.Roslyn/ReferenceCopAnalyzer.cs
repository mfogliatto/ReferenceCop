﻿namespace ReferenceCop
{
    using System;
    using System.Collections.Immutable;
    using System.Diagnostics;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReferenceCopAnalyzer : DiagnosticAnalyzer
    {
        private const string LaunchDebuggerKey = "build_property.LaunchDebugger";
        private const string RoslynDebuggerTriggerValue = "Roslyn";

        private IViolationDetector<AssemblyIdentity> detector;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.GeneralError,
        DiagnosticDescriptors.IllegalReferenceRule,
        DiagnosticDescriptors.DiscouragedReferenceRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterCompilationAction(compilationAnalysisContext =>
            {
                LaunchDebuggerIfRequested(compilationAnalysisContext);

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
            foreach (var violation in this.detector.GetViolationsFrom(compilation.ReferencedAssemblyNames))
            {
                compilationAnalysisContext.ReportDiagnostic(DiagnosticFactory.CreateFor(violation));
            }
        }
    }
}
