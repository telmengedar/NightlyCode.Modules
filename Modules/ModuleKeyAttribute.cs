using System;

namespace NightlyCode.Modules {

    /// <summary>
    /// attribute used to specify a key to identify a module
    /// </summary>
    public class ModuleKeyAttribute : Attribute {

        /// <summary>
        /// creates a new <see cref="ModuleKeyAttribute"/>
        /// </summary>
        /// <param name="key">key for module</param>
        public ModuleKeyAttribute(string key) {
            Key = key;
        }

        /// <summary>
        /// key for module
        /// </summary>
        public string Key { get; }
    }
}