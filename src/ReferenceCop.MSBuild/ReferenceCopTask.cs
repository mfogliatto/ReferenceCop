namespace ReferenceCop.MSBuild
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Build.Evaluation;
    using Microsoft.Build.Execution;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Logging;

    public class ReferenceCopTask : ITask
    {
        private const string ResolveProjectReferencesTarget = "ResolveProjectReferences";
        private const string MSBuildSourceProjectFileMetadataKey = "MSBuildSourceProjectFile";
        private const string ReferenceCopRepositoryRootProperty = "ReferenceCopRepositoryRoot";

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        [Required]
        public ITaskItem ProjectFile { get; set; }

        [Required]
        public string ConfigFilePath { get; set; }

        private IViolationDetector<string> tagViolationDetector;
        private IViolationDetector<string> pathViolationDetector;

        public bool Execute()
        {
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }

            bool success = true;
            try
            {
                var configLoader = new XmlConfigurationLoader(ConfigFilePath);
                var config = configLoader.Load();

                this.tagViolationDetector = new AssemblyTagViolationDetector(config, ProjectFile.ItemSpec, new AssemblyTagProvider());
                foreach (var violation in this.tagViolationDetector.GetViolationsFrom(GetProjectReferences()))
                {
                    success = false;
                    BuildEngine.LogViolation(violation, ProjectFile.ItemSpec);
                }

                var repositoryRoot = GetResolvedPropertyValue(ReferenceCopRepositoryRootProperty);
                this.pathViolationDetector = new AssemblyPathViolationDetector(config, ProjectFile.ItemSpec, new AssemblyPathProvider(repositoryRoot));
                foreach (var violation in this.pathViolationDetector.GetViolationsFrom(GetProjectReferences()))
                {
                    success = false;
                    BuildEngine.LogViolation(violation, ProjectFile.ItemSpec);
                }
            }
            catch (Exception ex)
            {
                success = false;
                BuildEngine.LogErrorEvent(ex);
            }

            return success;
        }

        private IEnumerable<string> GetProjectReferences()
        {
            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(ProjectFile.ItemSpec);

            var buildRequestData = new BuildRequestData(project.CreateProjectInstance(), new[] { ResolveProjectReferencesTarget });
            var buildParameters = new BuildParameters(projectCollection)
            {
                Loggers = new[] { new ConsoleLogger(LoggerVerbosity.Minimal) }
            };

            var buildResult = BuildManager.DefaultBuildManager.Build(buildParameters, buildRequestData);

            if (buildResult.OverallResult == BuildResultCode.Success)
            {
                var resolvedReferences = buildResult.ResultsByTarget[ResolveProjectReferencesTarget].Items;
                return resolvedReferences.Select(item => item.GetMetadata(MSBuildSourceProjectFileMetadataKey));
            }

            return Enumerable.Empty<string>();
        }

        private string GetResolvedPropertyValue(string propertyName)
        {
            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(ProjectFile.ItemSpec);
            project.ReevaluateIfNecessary();

            return project.GetPropertyValue(propertyName);
        }
    }
}
