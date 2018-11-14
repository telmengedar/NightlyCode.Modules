using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NightlyCode.Core.Conversion;
using NightlyCode.Modules.Commands;
using NightlyCode.Modules.Logging;

namespace NightlyCode.Modules {

    /// <summary>
    /// manages <see cref="IModule"/>s, provides access to them
    /// </summary>
    public class ModuleContext<TMetaInformation> : IModuleContext
        where TMetaInformation : ModuleInformation, new() {
        readonly object modulelock = new object();
        readonly Dictionary<Type, TMetaInformation> modules = new Dictionary<Type, TMetaInformation>();
        readonly Dictionary<string, IModule> modulekeylookup = new Dictionary<string, IModule>();

        readonly CommandParser parser = new CommandParser();
        readonly ModuleProvider provider = new ModuleProvider();

        public ModuleContext() {
            provider.Add(GetType());
        }

        /// <summary>
        /// triggered when a module has been started
        /// </summary>
        public event Action<IModule> ModuleStarted;

        /// <summary>
        /// triggered when a module has been stopped
        /// </summary>
        public event Action<IModule> ModuleStopped;

        /// <summary>
        /// loaded <see cref="IModule"/>s
        /// </summary>
        public IEnumerable<TMetaInformation> Modules => modules.Values;

        /// <summary>
        /// get meta information of a module
        /// </summary>
        /// <param name="moduletype">module of which to get information</param>
        /// <returns>module information</returns>
        public TMetaInformation GetInformation(Type moduletype) {
            lock(modulelock) {
                if(!modules.TryGetValue(moduletype, out TMetaInformation information))
                    throw new ModuleNotFoundException("Module is not registered to the module manager");
                return information;
            }
        }

        /// <summary>
        /// retrieve a key module from this context
        /// </summary>
        /// <typeparam name="T">type of <see cref="IModule"/> to retrieve</typeparam>
        /// <param name="key">key of module</param>
        /// <returns><see cref="IModule"/> with the specified key</returns>
        public T GetModuleByKey<T>(string key)
            where T : class, IModule
        {
            lock(modulelock) {
                if(!modulekeylookup.TryGetValue(key, out IModule module))
                    throw new ModuleNotFoundException($"Module with key '{key}' was not found");
                return (T)module;
            }
        }

        /// <summary>
        /// get module from context
        /// </summary>
        /// <typeparam name="T">type of module</typeparam>
        /// <returns>module if found</returns>
        public T GetModule<T>()
            where T : class, IModule {
            return (T) provider.Get<T>();
        }

        /// <summary>
        /// get meta information related to module
        /// </summary>
        /// <param name="moduletype">module of which to return meta information</param>
        /// <returns>meta information of module</returns>
        public TMetaInformation GetModuleInformation(Type moduletype) {
            return modules[moduletype];
        }

        /// <summary>
        /// adds an <see cref="IModule"/> to the context
        /// </summary>
        /// <remarks>
        /// this creates all metainformationen needed for the context for module management
        /// </remarks>
        /// <param name="moduletype">module to add</param>
        public void AddModule(Type moduletype) {
            provider.Add(moduletype);
            TMetaInformation information = CreateModuleInformation(moduletype);
            lock(modulelock)
                modules[moduletype] = information;
            OnModuleAdded(moduletype);
        }

        /// <summary>
        /// creates meta information for the specified module
        /// </summary>
        /// <param name="moduletype">module for which to create meta information</param>
        /// <returns>metainformation for the module</returns>
        protected virtual TMetaInformation CreateModuleInformation(Type moduletype) {
            TMetaInformation metainformation = new TMetaInformation {
                Type = moduletype,
                Key = moduletype.GetCustomAttribute<ModuleKeyAttribute>()?.Key,
                IsInitializable = moduletype.GetInterface(nameof(IInitializableModule)) != null
            };
            return metainformation;
        }

        internal void RecheckModuleState()
        {
            lock (modulelock)
            {
                foreach (TMetaInformation module in modules.Values)
                {
                    if (module.IsActivatable)
                    {
                        if (module.Status>=ModuleStatus.Started)
                            continue;

                        try
                        {
                            StartModule(module);
                        }
                        catch (Exception)
                        {
                            Logger.Warning(this, "Unable to activate module");
                        }
                    }
                    else
                    {
                        if (module.Status<ModuleStatus.Started)
                            continue;

                        Logger.Info(this, $"Stopping '{module.Type}'");
                        try
                        {
                            StopModule(module);
                        }
                        catch (Exception)
                        {
                            Logger.Warning(this, "Unable to stop module");
                        }
                    }
                }
            }
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

            IModule module = GetModuleByKey<IModule>(command.Module);
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
        /// starts a module manually
        /// </summary>
        /// <typeparam name="T">type of module</typeparam>
        public void StartModule<T>()
            where T : class, IModule {
            StartModule(modules[GetModule<T>()]);
        }

        /// <summary>
        /// stops a module manually
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void StopModule<T>()
            where T : class, IModule {
            StopModule(modules[GetModule<T>()]);
        }

        /// <summary>
        /// starts a module
        /// </summary>
        /// <param name="module">module to start</param>
        protected virtual void StartModule(ModuleInformation module) {
            if(module.Status>=ModuleStatus.Started)
                return;

            foreach(ModuleInformation dependency in module.Dependencies)
                StartModule(dependency);

            Logger.Info(this, $"Activating '{module.Type}'");
            try {
                (module.Module as IRunnableModule)?.Start();
                module.Status=ModuleStatus.Started;
            }
            catch (Exception e) {
                Logger.Error(this, $"Unable to start '{module.Type}'", e);
                module.Status=ModuleStatus.ErrorStarting;
                throw;
            }

            try {
                ModuleStarted?.Invoke(module.Module);
            }
            catch(Exception e) {
                Logger.Error(this, $"Error triggering {nameof(ModuleStopped)} event", e);
            }
        }

        /// <summary>
        /// stops the module
        /// </summary>
        /// <param name="module">module to stop</param>
        protected virtual void StopModule(ModuleInformation module) {
            if(module.Status<ModuleStatus.Started)
                return;

            foreach(ModuleInformation backdependency in module.BackDependencies)
                StopModule(backdependency);

            Logger.Info(this, $"Stopping '{module.Type}'");
            try {
                (module.Module as IRunnableModule)?.Stop();
                module.Status = ModuleStatus.Stopped;
            }
            catch (Exception e) {
                Logger.Error(this, $"Unable to stop '{module.Type}'", e);
                throw;
            }

            try {
                ModuleStopped?.Invoke(module.Module);
            }
            catch(Exception e) {
                Logger.Error(this, $"Error triggering {nameof(ModuleStopped)} event", e);
            }
            
        }

        /// <summary>
        /// start all managed modules
        /// </summary>
        public virtual void Start() {
            InitializeModules();
            RecheckModuleState();
        }

        /// <summary>
        /// stop all managed modules
        /// </summary>
        public virtual void Stop() {
            foreach (TMetaInformation module in modules.Values)
            {
                try
                {
                    StopModule(module);
                }
                catch (Exception) {
                    Logger.Warning(this, $"Unable to stop '{module.Type}', trying to continue work");
                }
            }
        }

        /// <summary>
        /// called when a module was added to the <see cref="ModuleContext{TMetaInformation}"/>
        /// </summary>
        /// <param name="moduletype">module which was added to the <see cref="ModuleContext{TMetaInformation}"/></param>
        protected virtual void OnModuleAdded(Type moduletype) { }
    }
}