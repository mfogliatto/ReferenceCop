namespace ReferenceCop
{
    /// <summary>
    /// A no-op trace writer that discards all messages. Used when tracing is disabled.
    /// </summary>
    public class NullTraceWriter : ITraceWriter
    {
        public static readonly NullTraceWriter Instance = new NullTraceWriter();

        public bool IsEnabled => false;

        public void Write(string message)
        {
            // Intentionally empty.
        }
    }
}
