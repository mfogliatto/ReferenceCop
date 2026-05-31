namespace ReferenceCop
{
    /// <summary>
    /// Interface for writing trace messages during rule evaluation.
    /// </summary>
    public interface ITraceWriter
    {
        /// <summary>
        /// Writes a trace message.
        /// </summary>
        /// <param name="message">The trace message.</param>
        void Write(string message);

        /// <summary>
        /// Gets a value indicating whether tracing is enabled.
        /// </summary>
        bool IsEnabled { get; }
    }
}
