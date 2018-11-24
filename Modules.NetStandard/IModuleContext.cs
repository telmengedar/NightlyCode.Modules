using System;
using System.Collections.Generic;

namespace NightlyCode.Modules {

    /// <summary>
    /// interface for a module context
    /// </summary>
    public interface IModuleContext {

        /// <summary>
        /// loaded modules
        /// </summary>
        IEnumerable<ModuleInformation> Modules { get; }

        /// <summary>
        /// retrieve a key module from this context
        /// </summary>
        /// <typeparam name="T">type of module to retrieve</typeparam>
        /// <param name="key">key of module</param>
        /// <returns>module with the specified key</returns>
        T GetModuleByKey<T>(string key);

        /// <summary>
        /// get module from context
        /// </summary>
        /// <typeparam name="T">type of module</typeparam>
        /// <returns>module if found</returns>
        T GetModule<T>();

        /// <summary>
        /// get meta information about module
        /// </summary>
        /// <returns>module meta information</returns>
        ModuleInformation GetModuleInformation(Type moduletype);

        /// <summary>
        /// executes a module command
        /// </summary>
        /// <param name="module">module which should execute the command</param>
        /// <param name="command">command to execute</param>
        /// <param name="arguments">command arguments</param>
        void ExecuteCommand(string module, string command, params string[] arguments);

        /// <summary>
        /// adds an module to the context
        /// </summary>
        /// <remarks>
        /// this creates all metainformationen needed for the context for module management
        /// </remarks>
        /// <param name="moduletype">module to add</param>
        /// <param name="instanceprovider">custom provider of module instance</param>
        void AddModule(Type moduletype, Func<ModuleProvider, object> instanceprovider=null);

        /// <summary>
        /// get module from context
        /// </summary>
        /// <param name="type">type of module</param>
        /// <returns>module if found</returns>
        object GetModule(Type type);
    }
}