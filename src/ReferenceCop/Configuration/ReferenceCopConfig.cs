namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot]
    public class ReferenceCopConfig
    {
        public ReferenceCopConfig()
        {
            this.Rules = new List<Rule>();
            this.UseExperimentalDetectors = false;
            this.EnableDebugMessages = false;
        }

        [XmlElement]
        public bool UseExperimentalDetectors { get; set; }

        [XmlElement]
        public bool EnableDebugMessages { get; set; }

        [XmlArrayItem(typeof(AssemblyName))]
        [XmlArrayItem(typeof(ProjectTag))]
        [XmlArrayItem(typeof(ProjectPath))]
        public List<Rule> Rules { get; set; }

        [Serializable]
        [XmlInclude(typeof(AssemblyName))]
        [XmlInclude(typeof(ProjectTag))]
        [XmlInclude(typeof(ProjectPath))]
        public abstract class Rule
        {
            public enum ViolationSeverity
            {
                None,
                Error,
                Warning,
            }

            public Rule()
            {
                this.Exceptions = new List<ProjectException>();
            }

            [XmlElement]
            public string Name { get; set; }

            [XmlElement]
            public string Description { get; set; }

            [XmlElement]
            public ViolationSeverity Severity { get; set; }

            [XmlArray]
            [XmlArrayItem("Project")]
            public List<ProjectException> Exceptions { get; set; }

            /// <summary>
            /// Determines whether the specified project is exempt from this rule.
            /// </summary>
            /// <param name="projectName">The project name to check.</param>
            /// <returns>True if the project is exempt; otherwise false.</returns>
            public bool IsProjectExempt(string projectName)
            {
                if (string.IsNullOrEmpty(projectName) || this.Exceptions == null || this.Exceptions.Count == 0)
                {
                    return false;
                }

                foreach (var exception in this.Exceptions)
                {
                    if (string.Equals(exception.Name, projectName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Serializable]
        public class ProjectException
        {
            [XmlAttribute]
            public string Name { get; set; }
        }

        [Serializable]
        public class AssemblyName : Rule
        {
            public string Pattern { get; set; }
        }

        [Serializable]
        public class ProjectTag : Rule
        {
            public string FromProjectTag { get; set; }

            public string ToProjectTag { get; set; }
        }

        [Serializable]
        public class ProjectPath : Rule
        {
            public string FromPath { get; set; }

            public string ToPath { get; set; }
        }
    }
}
