using System.Diagnostics.Tracing;

namespace NightlyCode.Modules {

    /// <summary>
    /// status of an module
    /// </summary>
    public enum ModuleStatus {

        /// <summary>
        /// not determined yet
        /// </summary>
        None,

        /// <summary>
        /// there was an error starting the module
        /// </summary>
        ErrorStarting,

        /// <summary>
        /// module was stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// module is started
        /// </summary>
        Started
    }
}