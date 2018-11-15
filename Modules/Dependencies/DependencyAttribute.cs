using System;

namespace NightlyCode.Modules.Dependencies {

    /// <summary>
    /// attribute used to specify a dependency to a <see cref="IModule"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
        /// <summary>
        /// creates a new <see cref="DependencyAttribute"/>
        /// </summary>
        /// <param name="module">module to which a dependency exists</param>
        /// <param name="specifiertype">type of module specifier</param>
        /// <param name="dependencytype">type of dependency</param>
        public DependencyAttribute(string module, SpecifierType specifiertype=SpecifierType.Type, DependencyType dependencytype=DependencyType.InitializeAfter)
        {
            Module = module;
            DependencyType = dependencytype;
            SpecifierType = specifiertype;
        }

        /// <summary>
        /// module to which a dependency exists
        /// </summary>
        public string Module { get; private set; }

        /// <summary>
        /// type of dependency
        /// </summary>
        public DependencyType DependencyType { get; set; }

        /// <summary>
        /// type of dependency specification
        /// </summary>
        public SpecifierType SpecifierType { get; private set; }
    }
}