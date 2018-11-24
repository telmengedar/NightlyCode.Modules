using System;

namespace NightlyCode.Modules {

    /// <summary>
    /// runtime information about an modules
    /// </summary>
    public class ModuleInformation {
         
        /// <summary>
        /// creates new <see cref="ModuleInformation"/>
        /// </summary>
        /// <param name="type">type name of module</param>
        public ModuleInformation(Type type) {
            Type = type;
            if (Attribute.IsDefined(type, typeof(ModuleAttribute)))
                Key = ((ModuleAttribute) Attribute.GetCustomAttribute(type, typeof(ModuleAttribute))).Key;
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
        /// provider used to create type
        /// </summary>
        public Func<ModuleProvider, object> Provider { get; internal set; }

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