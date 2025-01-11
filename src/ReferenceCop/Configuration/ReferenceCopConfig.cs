namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot]
    public class ReferenceCopConfig
    {
        [XmlArrayItem(typeof(AssemblyName))]
        [XmlArrayItem(typeof(AssemblyTag))]
        [XmlArrayItem(typeof(ProjectPath))]
        public List<Rule> Rules { get; set; }

        public ReferenceCopConfig()
        {
            this.Rules = new List<Rule>();
        }

        [Serializable]
        [XmlInclude(typeof(AssemblyName))]
        [XmlInclude(typeof(AssemblyTag))]
        [XmlInclude(typeof(ProjectPath))]
        public abstract class Rule
        {
            [XmlElement]
            public string Name { get; set; }
            [XmlElement]
            public string Description { get; set; }
            [XmlElement]
            public ViolationSeverity Severity { get; set; }

            public enum ViolationSeverity
            {
                None,
                Error,
                Warning
            }
        }

        [Serializable]
        public class AssemblyName : Rule
        {
            public string Pattern { get; set; }
        }

        [Serializable]
        public class AssemblyTag : Rule
        {
            public string FromAssemblyTag { get; set; }
            public string ToAssemblyTag { get; set; }
        }

        [Serializable]
        public class ProjectPath : Rule
        {
            public string FromPath { get; set; }
            public string ToPath { get; set; }
        }
    }
}