using System;
using System.Runtime.Serialization;

namespace NightlyCode.Modules.Commands {
    public class ModuleCommandException : Exception {
        public ModuleCommandException(string message)
            : base(message) {}

        public ModuleCommandException(string message, Exception innerException)
            : base(message, innerException) {}

        protected ModuleCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}