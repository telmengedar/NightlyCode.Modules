using System;

namespace NightlyCode.Modules {

    /// <summary>
    /// exception when creating modules
    /// </summary>
    public class ModuleCreateException : Exception {

        /// <summary>
        /// creates a new <see cref="ModuleCreateException"/>
        /// </summary>
        /// <param name="message">message for error</param>
        /// <param name="type">type which could not get created</param>
        /// <param name="innerException">exception which lead to that exception</param>
        public ModuleCreateException(string message, Type type, Exception innerException=null)
            : base(message, innerException) {
            ModuleType = type;
        }

        /// <summary>
        /// type which could not get instantiated
        /// </summary>
        public Type ModuleType { get; }
    }
}