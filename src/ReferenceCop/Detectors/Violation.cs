using System.Data;

namespace ReferenceCop
{
    public class Violation
    {
        public const string ViolationSeverityErrorCode = "RC0001";
        public const string ViolationSeverityWarningCode = "RC0002";

        public ReferenceCopConfig.Rule Rule { get; }
        public string ReferenceName { get; }

        public string Code
        {
            get
            {
                switch (Rule.Severity)
                {
                    case ReferenceCopConfig.Rule.ViolationSeverity.Error:
                        return ViolationSeverityErrorCode;
                    case ReferenceCopConfig.Rule.ViolationSeverity.Warning:
                        return ViolationSeverityWarningCode;
                    default:
                        return null;
                }
            }
        }

        public Violation(ReferenceCopConfig.Rule rule, string referenceName)
        {
            this.Rule = rule;
            this.ReferenceName = referenceName;
        }
    }
}
