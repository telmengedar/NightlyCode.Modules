using System;
using System.Collections.Generic;
using System.Linq;

namespace NightlyCode.Modules {

    /// <summary>
    /// runtime information about an <see cref="IModule"/>
    /// </summary>
    public class ModuleInformation {
         
        /// <summary>
        /// creates new <see cref="ModuleInformation"/>
        /// </summary> 
        public ModuleInformation() { }

        /// <summary>
        /// creates new <see cref="ModuleInformation"/>
        /// </summary>
        /// <param name="type">type name of module</param>
        public ModuleInformation(Type type) {
            Type = type;
            if (Attribute.IsDefined(type, typeof(ModuleKeyAttribute)))
                Key = ((ModuleKeyAttribute) Attribute.GetCustomAttribute(type, typeof(ModuleKeyAttribute))).Key;
            IsInitializable = Type.GetInterface(nameof(IInitializableModule)) != null;
        }

        public Type Type { get; internal set; }

        /// <summary>
        /// module key (if any)
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        /// name of module type
        /// </summary>
        public string TypeName => Type.Name;

        /// <summary>
        /// determines whether the module is initializable
        /// </summary>
        public bool IsInitializable { get; internal set; }

        /// <summary>
        /// status of module
        /// </summary>
        public ModuleStatus Status { get; internal set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString() {
            return TypeName;
        }
    }
}