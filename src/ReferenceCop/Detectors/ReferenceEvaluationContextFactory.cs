namespace ReferenceCop
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Factory for creating ReferenceEvaluationContext instances.
    /// </summary>
    public static class ReferenceEvaluationContextFactory
    {
        /// <summary>
        /// Creates a ReferenceEvaluationContext for the given reference.
        /// </summary>
        /// <typeparam name="TAssemblyIdentity">The type of assembly identity.</typeparam>
        /// <param name="reference">The reference.</param>
        /// <param name="noWarnCodes">The NoWarn codes associated with this reference.</param>
        /// <returns>A new ReferenceEvaluationContext instance.</returns>
        public static ReferenceEvaluationContext<TAssemblyIdentity> Create<TAssemblyIdentity>(
            TAssemblyIdentity reference,
            IEnumerable<string> noWarnCodes = null)
        {
            // Determine if ReferenceCop warnings are suppressed for this reference
            var isWarningSuppressed = false;
            if (noWarnCodes != null)
            {
                isWarningSuppressed = noWarnCodes.Contains(Violation.ViolationSeverityWarningCode);
            }

            return new ReferenceEvaluationContext<TAssemblyIdentity>(reference, isWarningSuppressed);
        }
    }
}
