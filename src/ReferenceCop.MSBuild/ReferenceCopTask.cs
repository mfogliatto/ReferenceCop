﻿namespace ReferenceCop.MSBuild
{
    using System;
    using System.Diagnostics;
    using Microsoft.Build.Framework;

    public class ReferenceCopTask : ITask
    {
        private const string ReferenceCopRepositoryRootProperty = "ReferenceCopRepositoryRoot";
        private const string MSBuildDebuggerTriggerValue = "MSBuild";

        private readonly IProjectMetadataProvider projectReferencesProvider;
        private readonly Func<string, IConfigurationLoader> configLoaderFactory;
        private readonly Func<ReferenceCopConfig, string, IViolationDetector<string>> tagViolationDetectorFactory;
        private readonly Func<ReferenceCopConfig, string, string, IViolationDetector<string>> pathViolationDetectorFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCopTask"/> class.
        /// The constructor for the ReferenceCopTask used by MSBuild.
        /// </summary>
        public ReferenceCopTask()
            : this(new MSBuildProjectMetadataProvider(), null, null, null)
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
        {
            this.projectReferencesProvider = projectReferencesProvider;

            this.configLoaderFactory = (configFilePaths) => configLoader ?? new XmlConfigurationLoader(configFilePaths);

            this.tagViolationDetectorFactory = (config, projectPath) =>
                tagViolationDetector ?? new ProjectTagViolationDetector(config, projectPath, new ProjectTagProvider());

            this.pathViolationDetectorFactory = (config, projectPath, repositoryRoot) =>
                pathViolationDetector ?? new ProjectPathViolationDetector(config, projectPath, new ProjectPathProvider(repositoryRoot));
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
                var projectReferences = this.projectReferencesProvider.GetProjectReferences(this.ProjectFile.ItemSpec);

                var projectTagViolationDetector = this.tagViolationDetectorFactory(config, this.ProjectFile.ItemSpec);
                foreach (var violation in projectTagViolationDetector.GetViolationsFrom(projectReferences))
                {
                    if (violation.Rule.Severity == ReferenceCopConfig.Rule.ViolationSeverity.Error)
                    {
                        success = false;
                    }

                    this.BuildEngine.LogViolation(violation, this.ProjectFile.ItemSpec);
                }

                var repositoryRoot = this.projectReferencesProvider.GetPropertyValue(
                    this.ProjectFile.ItemSpec, ReferenceCopRepositoryRootProperty);
                var projectPathViolationDetector = this.pathViolationDetectorFactory(config, this.ProjectFile.ItemSpec, repositoryRoot);
                foreach (var violation in projectPathViolationDetector.GetViolationsFrom(projectReferences))
                {
                    if (violation.Rule.Severity == ReferenceCopConfig.Rule.ViolationSeverity.Error)
                    {
                        success = false;
                    }

                    this.BuildEngine.LogViolation(violation, this.ProjectFile.ItemSpec);
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
