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
        private const string ProjectReferenceNode = "ProjectReference";

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

                var projectReferences = GetProjectReferencesFromCsproj();

                this.tagViolationDetector = new ProjectTagViolationDetector(config, ProjectFile.ItemSpec, new ProjectTagProvider());
                foreach (var violation in this.tagViolationDetector.GetViolationsFrom(projectReferences))
                {
                    success = false;
                    BuildEngine.LogViolation(violation, ProjectFile.ItemSpec);
                }

                var repositoryRoot = GetResolvedPropertyValue(ReferenceCopRepositoryRootProperty);
                this.pathViolationDetector = new ProjectPathViolationDetector(config, ProjectFile.ItemSpec, new ProjectPathProvider(repositoryRoot));
                foreach (var violation in this.pathViolationDetector.GetViolationsFrom(projectReferences))
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

        /// <summary>
        /// Gets the project references using MSBuild.
        /// </summary>
        /// <remarks>The returned collection includes transitive ProjectReferences. Currently unused, only kept for historical purposes.</remarks>
        /// <returns>The collection of ProjectReferences.</returns>
        private IEnumerable<string> GetProjectReferencesFromBuild()
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

        /// <summary>
        /// Gets the project references from the .csproj file.
        /// </summary>
        /// <remarks>The returned collection includes only direct ProjectReferences.</remarks>
        /// <returns>The collection of ProjectReferences.</returns>
        private List<string> GetProjectReferencesFromCsproj()
        {
            var projectCollection = new ProjectCollection();
            var project = projectCollection.LoadProject(ProjectFile.ItemSpec);

            // Get all ProjectReference items. These are the direct project references.
            var projectReferences = project.GetItems(ProjectReferenceNode);

            // Extract the Include attribute, which contains the path to the referenced project.
            return projectReferences.Select(pr => pr.EvaluatedInclude).ToList();
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
