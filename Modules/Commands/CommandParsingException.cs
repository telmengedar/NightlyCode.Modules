using System;
using System.Runtime.Serialization;

namespace NightlyCode.Modules.Commands {
    public class CommandParsingException : Exception {
        public CommandParsingException(string message)
            : base(message) {}

        public CommandParsingException(string message, Exception innerException)
            : base(message, innerException) {}

        protected CommandParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}