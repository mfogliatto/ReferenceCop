namespace ReferenceCop
{
    /// <summary>
    /// Represents a context for evaluating references, including warning suppression information.
    /// </summary>
    /// <typeparam name="TAssemblyIdentity">The type of assembly identity.</typeparam>
    public class ReferenceEvaluationContext<TAssemblyIdentity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceEvaluationContext{TAssemblyIdentity}"/> class.
        /// </summary>
        /// <param name="reference">The reference assembly identity.</param>
        /// <param name="isWarningSuppressed">Indicates whether warnings are suppressed for this reference.</param>
        public ReferenceEvaluationContext(TAssemblyIdentity reference, bool isWarningSuppressed = false)
        {
            this.Reference = reference;
            this.IsWarningSuppressed = isWarningSuppressed;
        }

        /// <summary>
        /// Gets the reference assembly identity.
        /// </summary>
        public TAssemblyIdentity Reference { get; }

        /// <summary>
        /// Gets a value indicating whether warnings are suppressed for this reference.
        /// </summary>
        public bool IsWarningSuppressed { get; }

        /// <summary>
        /// Returns a string representation of this reference evaluation context.
        /// </summary>
        /// <returns>A string containing the reference identity and warning suppression status.</returns>
        public override string ToString()
        {
            string referenceString = this.Reference?.ToString() ?? "null";
            string suppressionStatus = this.IsWarningSuppressed ? "Warnings Suppressed" : "Warnings Enabled";
            return $"Reference: {referenceString} ({suppressionStatus})";
        }
    }
}
