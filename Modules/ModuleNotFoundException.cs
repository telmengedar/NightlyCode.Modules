using System;
using System.Runtime.Serialization;

namespace NightlyCode.Modules {

    /// <summary>
    /// thrown when a requested module was not found
    /// </summary>
    public class ModuleNotFoundException : Exception {
        public ModuleNotFoundException(string message)
            : base(message) {}

        public ModuleNotFoundException(string message, Exception innerException)
            : base(message, innerException) {}

        protected ModuleNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) {}
    }
}