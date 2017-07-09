using System;

namespace NightlyCode.Modules {

    /// <summary>
    /// attribute used to specify a dependency to a <see cref="IModule"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {

        /// <summary>
        /// creates a new <see cref="DependencyAttribute"/>
        /// </summary>
        /// <param name="dependency"></param>
        /// <param name="type"></param>
        public DependencyAttribute(string dependency, DependencyType type)
        {
            Dependency = dependency;
            Type = type;
        }

        /// <summary>
        /// dependency
        /// </summary>
        public string Dependency { get; private set; }

        /// <summary>
        /// type of dependency specification
        /// </summary>
        public DependencyType Type { get; private set; }
    }
}