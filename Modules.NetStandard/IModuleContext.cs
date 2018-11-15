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
        /// executes a module command
        /// </summary>
        /// <param name="module">module which should execute the command</param>
        /// <param name="command">command to execute</param>
        /// <param name="arguments">command arguments</param>
        void ExecuteCommand(string module, string command, params string[] arguments);
    }
}