namespace ReferenceCop.MSBuild
{
    using System;
    using Microsoft.Build.Framework;

    internal static class BuildEngineExtensions
    {
        private const string SenderName = "ReferenceCop";

        public static void LogViolation(this IBuildEngine self, Violation violation, string file)
        {
            switch (violation.Rule.Severity)
            {
                case ReferenceCopConfig.Rule.ViolationSeverity.Error:
                    {
                        var errorEvent = CreateErrorEventFor(violation, file);
                        self.LogErrorEvent(errorEvent);
                        break;
                    }

                case ReferenceCopConfig.Rule.ViolationSeverity.Warning:
                    {
                        var warningEvent = CreateWarningEventFor(violation, file);
                        self.LogWarningEvent(warningEvent);
                        break;
                    }
            }
        }

        internal static void LogErrorEvent(this IBuildEngine self, Exception ex)
        {
            var errorEvent = CreateErrorEventFor(ex);
            self.LogErrorEvent(errorEvent);
        }

        internal static BuildErrorEventArgs CreateErrorEventFor(Exception ex)
        {
            return new BuildErrorEventArgs(
                        subcategory: SenderName,
                        code: "RC0000",
                        file: default,
                        lineNumber: default,
                        columnNumber: default,
                        endLineNumber: default,
                        endColumnNumber: default,
                        $"An error occurred while executing the MSBuild task: {ex.Message}",
                        helpKeyword: default,
                        senderName: SenderName);
        }

        internal static BuildErrorEventArgs CreateErrorEventFor(Violation violation, string file)
        {
            return new BuildErrorEventArgs(
                        subcategory: SenderName,
                        violation.Code,
                        file: file,
                        lineNumber: default,
                        columnNumber: default,
                        endLineNumber: default,
                        endColumnNumber: default,
                        ViolationMessageTemplates.GetIllegalReferenceMessage(violation.ReferenceName, violation.Rule.Name, violation.Rule.Description),
                        helpKeyword: default,
                        senderName: SenderName);
        }

        internal static BuildWarningEventArgs CreateWarningEventFor(Violation violation, string file)
        {
            return new BuildWarningEventArgs(
                        subcategory: SenderName,
                        violation.Code,
                        file: file,
                        lineNumber: default,
                        columnNumber: default,
                        endLineNumber: default,
                        endColumnNumber: default,
                        ViolationMessageTemplates.GetDiscouragedReferenceMessage(violation.ReferenceName, violation.Rule.Name, violation.Rule.Description),
                        helpKeyword: default,
                        senderName: SenderName);
        }
    }
}
