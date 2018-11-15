using System;

namespace NightlyCode.Modules {

    /// <summary>
    /// marks classes to be detected as module by the module context
    /// </summary>
    public class ModuleAttribute : Attribute {

        /// <summary>
        /// determines whether to automatically create the module when module context is started
        /// </summary>
        public bool AutoCreate { get; set; }

        /// <summary>
        /// key to use for module
        /// </summary>
        public string Key { get; set; }
    }
}