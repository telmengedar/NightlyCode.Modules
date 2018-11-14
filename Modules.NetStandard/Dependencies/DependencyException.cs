using System;

namespace NightlyCode.Modules.Dependencies {

    /// <summary>
    /// exception thrown when dependencies can't get resolved
    /// </summary>
    public class DependencyException : Exception {

        /// <summary>
        /// creates a new <see cref="DependencyException"/>
        /// </summary>
        /// <param name="message">message for exception to contain</param>
        public DependencyException(string message) : base(message) { }
    }
}