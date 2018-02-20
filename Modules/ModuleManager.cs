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
    public class ModuleManager<TMetaInformation> : IModuleContext
        where TMetaInformation : ModuleInformation, new() {
        readonly object modulelock = new object();
        readonly Dictionary<IModule, TMetaInformation> modules = new Dictionary<IModule, TMetaInformation>();
        readonly Dictionary<string, IModule> modulekeylookup = new Dictionary<string, IModule>();
        readonly Dictionary<Type, IModule> moduletypelookup = new Dictionary<Type, IModule>();

        readonly CommandParser parser = new CommandParser();

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
        /// <param name="module">module of which to get information</param>
        /// <returns>module information</returns>
        public TMetaInformation GetInformation(IModule module) {
            lock(modulelock) {
                TMetaInformation information;
                if(!modules.TryGetValue(module, out information))
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
                IModule module;
                if(!modulekeylookup.TryGetValue(key, out module))
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
            where T : class, IModule
        {
            lock(modulelock) {
                IModule module;
                if (!moduletypelookup.TryGetValue(typeof(T), out module))
                    throw new ModuleNotFoundException($"Module with type '{typeof(T)}' was not found");
                return (T)module;
            }
        }

        /// <summary>
        /// get meta information related to module
        /// </summary>
        /// <param name="module">module of which to return meta information</param>
        /// <returns>meta information of module</returns>
        public TMetaInformation GetModuleInformation(IModule module) {
            return modules[module];
        }

        /// <summary>
        /// adds an <see cref="IModule"/> to the context
        /// </summary>
        /// <remarks>
        /// this creates all metainformationen needed for the context for module management
        /// </remarks>
        /// <param name="module">module to add</param>
        public void AddModule(IModule module) {
            TMetaInformation information = CreateModuleInformation(module);
            lock(modulelock) {
                moduletypelookup[module.GetType()] = module;

                // determine whether this module has a key linked to it
                ModuleKeyAttribute keyattribute = module.GetType().GetCustomAttribute(typeof(ModuleKeyAttribute)) as ModuleKeyAttribute;
                if(keyattribute != null)
                    modulekeylookup[keyattribute.Key] = module;

                modules[module] = information;
            }
            OnModuleAdded(module);
        }

        /// <summary>
        /// creates meta information for the specified module
        /// </summary>
        /// <param name="module">module for which to create meta information</param>
        /// <returns>metainformation for the module</returns>
        protected virtual TMetaInformation CreateModuleInformation(IModule module) {
            TMetaInformation metainformation = new TMetaInformation {
                Key =  module.GetType().GetCustomAttribute<ModuleKeyAttribute>()?.Key,
                Type = module.GetType().Name,
                Module = module
            };
            return metainformation;
        }

        void InitializeModule(ModuleInformation module, HashSet<IModule> initialized) {
            if(initialized.Contains(module.Module))
                throw new Exception("Module already initialized. There probably is a circular reference somewhere.");

            if(module.Status>=ModuleStatus.Initialized)
                return;

            initialized.Add(module.Module);

            foreach (ModuleInformation dependency in module.Dependencies) {
                if (dependency.Module == null)
                    throw new Exception($"Module '{module.Module}' has unmet dependency to '{dependency}'");

                InitializeModule(dependency, initialized);
            }

            (module.Module as IInitializableModule)?.Initialize();
            module.Status=ModuleStatus.Initialized;

            initialized.Remove(module.Module);
        }

        void InitializeModules()
        {
            Logger.Info(this, "Initializing modules");
            foreach (TMetaInformation module in modules.Values)
                FillDependencies(module);

            HashSet<IModule> initialized = new HashSet<IModule>();
            foreach (TMetaInformation module in modules.Values) {
                if(module.Status>=ModuleStatus.Initialized)
                    continue;

                initialized.Clear();
                try
                {
                    Logger.Info(this, $"Initializing '{module.Type}'");
                    InitializeModule(module, initialized);
                }
                catch (Exception e)
                {
                    Logger.Error(this, $"Unable to initialize '{module.Type}'", e);
                    module.Status = ModuleStatus.ErrorInitializing;
                }
            }
        }

        /// <summary>
        /// fills dependencies of all modules
        /// </summary>
        /// <remarks>
        /// this is done after all modules have been added and before module initialization.
        /// Dependencies are used later for every activation
        /// </remarks>
        void FillDependencies(TMetaInformation module) {
            DependencyAttribute[] dependencies = Attribute.GetCustomAttributes(module.Module.GetType(), typeof(DependencyAttribute)) as DependencyAttribute[];
            if(dependencies == null)
                return;

            foreach(DependencyAttribute dependency in dependencies) {
                switch(dependency.Type) {
                    case DependencyType.Key:
                        IModule keymodule = GetModuleByKey<IModule>(dependency.Dependency);
                        ModuleInformation keyinformation = keymodule == null ? new ModuleInformation(dependency.Dependency, "", null) : modules[keymodule];
                        module.AddDependency(keyinformation);
                        keyinformation?.AddBackDependency(module);
                        break;
                    case DependencyType.Type:
                        IModule typemodule = modules.Values.Where(m => m.Type == dependency.Dependency).Select(m => m.Module).FirstOrDefault();
                        ModuleInformation typeinformation = typemodule == null ? new ModuleInformation("", dependency.Dependency, null) : modules[typemodule];
                        module.AddDependency(typeinformation);
                        typeinformation?.AddBackDependency(module);
                        break;
                    default:
                        throw new Exception("Invalid dependency type");
                }
            }
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
        /// called when a module was added to the <see cref="ModuleManager{TMetaInformation}"/>
        /// </summary>
        /// <param name="module">module which was added to the <see cref="ModuleManager{TMetaInformation}"/></param>
        protected virtual void OnModuleAdded(IModule module) { }
    }
}