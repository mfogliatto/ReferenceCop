namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A trace writer that collects trace messages in memory.
    /// Messages can be retrieved via the <see cref="Messages"/> property or
    /// forwarded to an external handler via the <see cref="OnMessage"/> callback.
    /// </summary>
    public class TraceWriter : ITraceWriter
    {
        private readonly List<string> messages = new List<string>();

        /// <summary>
        /// Gets a value indicating whether tracing is enabled.
        /// </summary>
        public bool IsEnabled => true;

        /// <summary>
        /// Gets the collected trace messages.
        /// </summary>
        public IReadOnlyList<string> Messages => this.messages;

        /// <summary>
        /// Gets or sets an optional callback invoked for each trace message.
        /// </summary>
        public Action<string> OnMessage { get; set; }

        /// <summary>
        /// Writes a trace message.
        /// </summary>
        /// <param name="message">The trace message.</param>
        public void Write(string message)
        {
            this.messages.Add(message);
            this.OnMessage?.Invoke(message);
        }
    }
}
