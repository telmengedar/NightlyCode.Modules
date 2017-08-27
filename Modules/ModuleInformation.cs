using System.Collections.Generic;
using System.Linq;

namespace NightlyCode.Modules {

    /// <summary>
    /// runtime information about an <see cref="IModule"/>
    /// </summary>
    public class ModuleInformation {
        readonly List<ModuleInformation> dependencies = new List<ModuleInformation>();
        readonly List<ModuleInformation> backdependencies=new List<ModuleInformation>();
         
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

        internal void AddBackDependency(ModuleInformation module) {
            backdependencies.Add(module);
        }

        internal void AddDependency(ModuleInformation module) {
            dependencies.Add(module);
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
        /// modules depending on this <see cref="IModule"/>
        /// </summary>
        /// <remarks>
        /// TODO: find a more suiting name
        /// </remarks>
        public IEnumerable<ModuleInformation> BackDependencies => backdependencies;
         
        /// <summary>
        /// status of module
        /// </summary>
        public ModuleStatus Status { get; internal set; }

        /// <summary>
        /// determines whether the module can get activated
        /// </summary>
        public virtual bool IsActivatable
        {
            get { return Module != null && Status>=ModuleStatus.Initialized && Dependencies.All(d => d.IsActivatable); }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            if(string.IsNullOrEmpty(Type))
                return Key;
            return Type;
        }
    }
}