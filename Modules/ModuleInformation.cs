using System.Collections.Generic;
using System.Linq;

namespace NightlyCode.Modules {

    /// <summary>
    /// runtime information about an <see cref="IModule"/>
    /// </summary>
    public class ModuleInformation {
        readonly List<ModuleInformation> dependencies = new List<ModuleInformation>();
        
        /// <summary>
        /// creates new <see cref="ModuleInformation"/>
        /// </summary> 
        public ModuleInformation() { }

        /// <summary>
        /// creates new <see cref="ModuleInformation"/>
        /// </summary>
        /// <param name="key">key of module</param>
        /// <param name="type">type name of module</param>
        /// <param name="module">module instance</param>
        public ModuleInformation(string key, string type, IModule module)
        {
            Key = key;
            Type = type;
            Module = module;
        }

        internal void AddDependency(ModuleInformation module) {
            dependencies.Add(module);
        }

        internal void SetInitialized(bool value)
        {
            IsInitialized = value;
        }

        internal void SetRunning(bool value)
        {
            IsRunning = value;
        }

        /// <summary>
        /// module key (if any)
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// name of module type
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// module instance
        /// </summary>
        public IModule Module { get; internal set; }

        /// <summary>
        /// dependencies of module
        /// </summary>
        public IEnumerable<ModuleInformation> Dependencies => dependencies;

        /// <summary>
        /// determines whether the module is initialized
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// determines whether the module is running
        /// </summary>
        public bool IsRunning { get; protected set; }

        /// <summary>
        /// determines whether the module can get activated
        /// </summary>
        public virtual bool IsActivatable
        {
            get { return Module != null && IsInitialized && Dependencies.All(d => d.IsRunning && d.IsActivatable); }
        }
    }
}