namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;
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

        // The confirmed working property keys for NoWarn support
        private const string HasNoWarnKey = "build_property.ReferenceCop_HasNoWarn";
        private const string NoWarnAssembliesKey = "build_property.ReferenceCop_NoWarnAssemblies";

        private IViolationDetector<AssemblyIdentity> assemblyNameViolationDetector;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
        DiagnosticDescriptors.GeneralError,
        DiagnosticDescriptors.IllegalReferenceRule,
        DiagnosticDescriptors.DiscouragedReferenceRule);

        public override void Initialize(AnalysisContext context)
        {
            // Enable concurrent execution
            context.EnableConcurrentExecution();
            
            // Configure generated code analysis
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationAction(compilationAnalysisContext =>
            {
                LaunchDebuggerIfRequested(compilationAnalysisContext);

                try
                {
                    var configLoader = new XmlConfigurationLoader(compilationAnalysisContext);
                    var config = configLoader.Load();
                    this.assemblyNameViolationDetector = new AssemblyNameViolationDetector(new PatternMatchComparer(), config);
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
            var noWarnByAssembly = this.GetNoWarnByAssembly(compilationAnalysisContext);

            var evaluationContexts = compilation.ReferencedAssemblyNames
                .Select(assemblyRef =>
                {
                    // Check if this assembly has NoWarn settings
                    var assemblyName = assemblyRef.Name;
                    var noWarnCodes = noWarnByAssembly.TryGetValue(assemblyName, out var codes) ? codes : null;
                    return ReferenceEvaluationContextFactory.Create(assemblyRef, noWarnCodes);
                })
                .ToList();

            foreach (var violation in this.assemblyNameViolationDetector.GetViolationsFrom(evaluationContexts))
            {
                compilationAnalysisContext.ReportDiagnostic(DiagnosticFactory.CreateFor(violation));
            }
        }

        private Dictionary<string, IEnumerable<string>> GetNoWarnByAssembly(CompilationAnalysisContext context)
        {
            var result = new Dictionary<string, IEnumerable<string>>();
            
            // Get HasNoWarn value - use the confirmed working property
            string hasNoWarnValue = null;
            context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(HasNoWarnKey, out hasNoWarnValue);
            
            if (string.IsNullOrEmpty(hasNoWarnValue) || !hasNoWarnValue.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }

            // Get NoWarnAssemblies value - use the confirmed working property
            string noWarnAssemblies = null;
            context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(NoWarnAssembliesKey, out noWarnAssemblies);

            if (!string.IsNullOrEmpty(noWarnAssemblies))
            {
                var entries = noWarnAssemblies.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    var parts = entry.Split(new[] { '|' }, 2);
                    if (parts.Length == 2)
                    {
                        var assemblyName = parts[0];
                        var noWarnCodes = parts[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                  .Select(code => code.Trim());
                        
                        result[assemblyName] = noWarnCodes;
                    }
                }
            }

            return result;
        }
    }
}
