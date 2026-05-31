namespace ReferenceCop.MSBuild
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Build.Framework;

    public class ReferenceCopTask : ITask
    {
        private const string ReferenceCopRepositoryRootProperty = "ReferenceCopRepositoryRoot";
        private const string MSBuildDebuggerTriggerValue = "MSBuild";

        private readonly IProjectMetadataProvider projectReferencesProvider;
        private readonly Func<string, IConfigurationLoader> configLoaderFactory;
        private readonly Func<ReferenceCopConfig, string, ITraceWriter, IViolationDetector<string>> projectTagViolationDetectorFactory;
        private readonly Func<ReferenceCopConfig, string, string, ITraceWriter, IViolationDetector<string>> projectPathViolationDetectorFactory;
        private readonly Func<bool, ITraceWriter> traceWriterFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCopTask"/> class.
        /// The constructor for the ReferenceCopTask used by MSBuild.
        /// </summary>
        public ReferenceCopTask()
            : this(new MSBuildProjectMetadataProvider(), null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCopTask"/> class.
        /// The constructor for the ReferenceCopTask used in unit tests.
        /// </summary>
        /// <param name="projectReferencesProvider">IProjectMetadataProvider.</param>
        /// <param name="configLoader">IConfigurationLoader.</param>
        /// <param name="tagViolationDetector">IViolationDetector.</param>
        /// <param name="pathViolationDetector">pathViolationDetector.</param>
        public ReferenceCopTask(
            IProjectMetadataProvider projectReferencesProvider,
            IConfigurationLoader configLoader,
            IViolationDetector<string> tagViolationDetector,
            IViolationDetector<string> pathViolationDetector)
            : this(projectReferencesProvider, configLoader, tagViolationDetector, pathViolationDetector, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCopTask"/> class.
        /// Full constructor supporting dependency injection of all components.
        /// </summary>
        public ReferenceCopTask(
            IProjectMetadataProvider projectReferencesProvider,
            IConfigurationLoader configLoader,
            IViolationDetector<string> tagViolationDetector,
            IViolationDetector<string> pathViolationDetector,
            Func<bool, ITraceWriter> traceWriterFactory)
        {
            this.projectReferencesProvider = projectReferencesProvider;

            this.configLoaderFactory = (configFilePaths) => configLoader ?? new XmlConfigurationLoader(configFilePaths);

            this.projectTagViolationDetectorFactory = (config, projectPath, tw) =>
                tagViolationDetector ?? new ProjectTagViolationDetector(config, projectPath, new ProjectTagProvider(), tw);

            this.projectPathViolationDetectorFactory = (config, projectPath, repositoryRoot, tw) =>
                pathViolationDetector ?? new ProjectPathViolationDetector(config, projectPath, new ProjectPathProvider(repositoryRoot), tw);

            this.traceWriterFactory = traceWriterFactory ?? (enableTracing =>
            {
                if (enableTracing)
                {
                    return new TraceWriter();
                }

                return NullTraceWriter.Instance;
            });
        }

        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        [Required]
        public ITaskItem ProjectFile { get; set; }

        [Required]
        public string ConfigFilePaths { get; set; }

        public string LaunchDebugger { get; set; }

        public bool Execute()
        {
            this.LaunchDebuggerIfRequested();

            bool success = true;
            try
            {
                var configFilePath = ConfigFilePathsParser.Parse(this.ConfigFilePaths);
                var configLoader = this.configLoaderFactory(configFilePath);
                var config = configLoader.Load();

                var traceWriter = this.traceWriterFactory(config.EnableTracing);

                if (config.EnableDebugMessages && config.UseExperimentalDetectors)
                {
                    this.BuildEngine.LogDebugMessage("Using experimental detectors");
                }

                if (config.EnableTracing)
                {
                    this.BuildEngine.LogDebugMessage("Tracing is enabled");
                }

                var projectReferences = this.projectReferencesProvider.GetProjectReferences(this.ProjectFile.ItemSpec);
                var evaluationContexts = projectReferences
                    .Select(_ => ReferenceEvaluationContextFactory.Create(_.Path, _.NoWarn))
                    .ToList();

                var projectTagViolationDetector = this.projectTagViolationDetectorFactory(config, this.ProjectFile.ItemSpec, traceWriter);
                var projectTagViolations = config.UseExperimentalDetectors
                    ? projectTagViolationDetector.GetViolationsFromExperimental(evaluationContexts)
                    : projectTagViolationDetector.GetViolationsFrom(evaluationContexts);

                foreach (var violation in projectTagViolations)
                {
                    if (violation.Rule.Severity == ReferenceCopConfig.Rule.ViolationSeverity.Error)
                    {
                        success = false;
                    }

                    this.BuildEngine.LogViolation(violation, this.ProjectFile.ItemSpec);
                }

                var repositoryRoot = this.projectReferencesProvider.GetPropertyValue(
                    this.ProjectFile.ItemSpec, ReferenceCopRepositoryRootProperty);
                var projectPathViolationDetector = this.projectPathViolationDetectorFactory(config, this.ProjectFile.ItemSpec, repositoryRoot, traceWriter);
                var projectPathViolations = config.UseExperimentalDetectors
                    ? projectPathViolationDetector.GetViolationsFromExperimental(evaluationContexts)
                    : projectPathViolationDetector.GetViolationsFrom(evaluationContexts);

                foreach (var violation in projectPathViolations)
                {
                    if (violation.Rule.Severity == ReferenceCopConfig.Rule.ViolationSeverity.Error)
                    {
                        success = false;
                    }

                    this.BuildEngine.LogViolation(violation, this.ProjectFile.ItemSpec);
                }

                // Flush trace messages to the build log
                if (traceWriter is TraceWriter tw)
                {
                    foreach (var message in tw.Messages)
                    {
                        this.BuildEngine.LogTraceMessage(message);
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                this.BuildEngine.LogErrorEvent(ex);
            }

            return success;
        }

        private void LaunchDebuggerIfRequested()
        {
            bool launchDebuggerRequested = !string.IsNullOrEmpty(this.LaunchDebugger) && this.LaunchDebugger.Contains(MSBuildDebuggerTriggerValue);
            if (!Debugger.IsAttached && launchDebuggerRequested)
            {
                Debugger.Launch();
            }
        }
    }
}
