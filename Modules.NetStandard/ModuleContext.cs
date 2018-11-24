using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NightlyCode.Core.Conversion;
using NightlyCode.Modules.Commands;
using NightlyCode.Modules.Logging;

namespace NightlyCode.Modules {

    /// <summary>
    /// manages modules, provides access to them
    /// </summary>
    public class ModuleContext : IModuleContext {
        readonly object modulelock = new object();
        readonly Dictionary<Type, ModuleInformation> modules = new Dictionary<Type, ModuleInformation>();
        readonly Dictionary<string, ModuleInformation> modulekeylookup = new Dictionary<string, ModuleInformation>();

        readonly CommandParser parser = new CommandParser();
        readonly ModuleProvider provider;

        /// <summary>
        /// creates a new <see cref="ModuleContext"/>
        /// </summary>
        public ModuleContext() {
            provider = new ModuleProvider(this);
            provider.AddInstance(this);
        }

        /// <summary>
        /// determines whether the specified module has been instantiated
        /// </summary>
        /// <typeparam name="T">type of module to check for</typeparam>
        /// <returns>true if module was instantiated, false otherwise</returns>
        public bool IsStarted<T>() {
            return IsStarted(typeof(T));
        }

        /// <summary>
        /// determines whether the specified module has been instantiated
        /// </summary>
        /// <param name="type">type of module to check for</param>
        /// <returns>true if module was instantiated, false otherwise</returns>
        public bool IsStarted(Type type) {
            return provider.HasInstance(type);
        }

        /// <summary>
        /// loaded modules
        /// </summary>
        public IEnumerable<ModuleInformation> Modules {
            get {
                lock(modulelock)
                    foreach (ModuleInformation module in modules.Values)
                        yield return module;
            }
        }

        /// <summary>
        /// get meta information of a module
        /// </summary>
        /// <param name="moduletype">module of which to get information</param>
        /// <returns>module information</returns>
        public ModuleInformation GetInformation(Type moduletype) {
            lock(modulelock) {
                if(!modules.TryGetValue(moduletype, out ModuleInformation information))
                    throw new ModuleNotFoundException("Module is not registered to the module manager");
                return information;
            }
        }

        /// <summary>
        /// retrieve a key module from this context
        /// </summary>
        /// <typeparam name="T">type of module to retrieve</typeparam>
        /// <param name="key">key of module</param>
        /// <returns>module with the specified key</returns>
        public T GetModuleByKey<T>(string key)
        {
            lock(modulelock) {
                if(!modulekeylookup.TryGetValue(key, out ModuleInformation module))
                    throw new ModuleNotFoundException($"Module with key '{key}' was not found");
                return (T)provider.Get(module.Type);
            }
        }

        /// <summary>
        /// get module from context
        /// </summary>
        /// <param name="type">type of module</param>
        /// <returns>module if found</returns>
        public object GetModule(Type type) {
            return provider.Get(type);
        }

        /// <summary>
        /// get module from context
        /// </summary>
        /// <typeparam name="T">type of module</typeparam>
        /// <returns>module if found</returns>
        public T GetModule<T>() {
            return (T) provider.Get<T>();
        }

        /// <summary>
        /// get meta information related to module
        /// </summary>
        /// <param name="moduletype">module of which to return meta information</param>
        /// <returns>meta information of module</returns>
        public ModuleInformation GetModuleInformation(Type moduletype) {
            lock (modulelock) {
                modules.TryGetValue(moduletype, out ModuleInformation info);
                return info;
            }
        }

        /// <summary>
        /// adds an module to the context
        /// </summary>
        /// <typeparam name="T">type of module to add</typeparam>
        public void AddModule<T>() {
            AddModule(typeof(T));
        }

        /// <summary>
        /// adds an module to the context
        /// </summary>
        /// <remarks>
        /// this creates all metainformationen needed for the context for module management
        /// </remarks>
        /// <param name="moduletype">module to add</param>
        /// <param name="instanceprovider">custom provider of module instance</param>
        public void AddModule(Type moduletype, Func<ModuleProvider, object> instanceprovider=null) {
            provider.Add(moduletype);
            ModuleInformation information = new ModuleInformation(moduletype) {
                Provider = instanceprovider
            };

            lock(modulelock) {
                modules[moduletype] = information;
                if(!string.IsNullOrEmpty(information.Key))
                    modulekeylookup[information.Key] = information;
            }

            OnModuleAdded(moduletype);
        }

        /// <summary>
        /// executes a command on a module
        /// </summary>
        /// <remarks>
        /// first argument has to be the module key, the other arguments are redirected to the module
        /// </remarks>
        /// <param name="modulekey">module which should execute the command</param>
        /// <param name="command">command to execute</param>
        /// <param name="arguments">arguments for command call</param>
        public void ExecuteCommand(string modulekey, string command, params string[] arguments) {
            ICommandModule module = GetModuleByKey<ICommandModule>(modulekey);
            module.ProcessCommand(command, arguments);
        }

        /// <summary>
        /// executes a command for a module
        /// </summary>
        /// <param name="commandstring">string which defines the command</param>
        public bool ExecuteCommand(string commandstring) {
            ModuleCommand command = parser.ParseCommand(commandstring);
            if(command.Type == CommandType.None)
                return false;

            object module = GetModuleByKey<object>(command.Module);
            switch(command.Type) {
                case CommandType.Property:
                    PropertyInfo property=module.GetType().GetProperties().First(p => p.Name.ToLower() == command.Endpoint.ToLower());
                    property.SetValue(module, Converter.Convert(command.Arguments[0], property.PropertyType));
                    return true;
                case CommandType.Method:
                    MethodInfo[] methods = module.GetType().GetMethods().Where(m => m.Name.ToLower() == command.Endpoint.ToLower() && m.GetParameters().Length == command.Arguments.Length).ToArray();
                    foreach(MethodInfo method in methods) {
                        try {
                            int index = 0;
                            ParameterInfo[] parameter = method.GetParameters();
                             method.Invoke(module, command.Arguments.Select(c => {
                                object value = Converter.Convert(command.Arguments[index], parameter[index].ParameterType);
                                ++index;
                                return value;
                            }).ToArray());
                            return true;
                        }
                        catch(Exception) {
                        }
                    }
                    throw new ModuleCommandException("No matching method found to call");
                default:
                    throw new ModuleCommandException($"Unknown command type '{command.Type}'");
            }
        }

        /// <summary>
        /// starts the module manager and autocreates all marked modules
        /// </summary>
        public virtual void Start() {
            foreach(ModuleInformation moduleinformation in Modules.Where(i => (Attribute.GetCustomAttribute(i.Type, typeof(ModuleAttribute)) as ModuleAttribute)?.AutoCreate ?? false).ToArray())
                try {
                    provider.Get(moduleinformation.Type);
                }
                catch (Exception e) {
                    Logger.Error(this, $"Unable to start '{moduleinformation.TypeName}'", e);
                }
        }

        /// <summary>
        /// stop all managed modules
        /// </summary>
        public virtual void Stop() {
            foreach(IDisposable disposable in provider.Instances.OfType<IDisposable>())
                disposable.Dispose();
        }

        /// <summary>
        /// called when a module was added to the <see cref="ModuleContext"/>
        /// </summary>
        /// <param name="moduletype">module which was added to the <see cref="ModuleContext"/></param>
        protected virtual void OnModuleAdded(Type moduletype) { }
    }
}