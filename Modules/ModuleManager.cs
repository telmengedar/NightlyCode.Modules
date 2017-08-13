using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NightlyCode.Modules {

    /// <summary>
    /// manages <see cref="IModule"/>s, provides access to them
    /// </summary>
    public class ModuleManager<TMetaInformation>
        where TMetaInformation : ModuleInformation, new() {
        readonly object modulelock = new object();
        readonly Dictionary<IModule, TMetaInformation> modules = new Dictionary<IModule, TMetaInformation>();
        readonly Dictionary<string, IModule> modulekeylookup = new Dictionary<string, IModule>();
        readonly Dictionary<Type, IModule> moduletypelookup = new Dictionary<Type, IModule>(); 
        
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

        void InitializeModules()
        {
            Logger.Info(this, "Initializing modules");
            foreach (TMetaInformation module in modules.Values.Where(m=>!m.IsInitialized))
            {
                try
                {
                    Logger.Info(this, $"Initializing '{module.Type}'");
                    FillDependencies(module);
                    (module.Module as IInitializableModule)?.Initialize();
                    module.SetInitialized(true);
                }
                catch (Exception e)
                {
                    Logger.Error(this, $"Unable to initialize '{module.Type}'", e);
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
            DependencyAttribute[] dependencies = Attribute.GetCustomAttributes(module.Module.GetType()) as DependencyAttribute[];
            if(dependencies == null)
                return;

            foreach(DependencyAttribute dependency in dependencies) {
                switch(dependency.Type) {
                    case DependencyType.Key:
                        IModule keymodule = GetModuleByKey<IModule>(dependency.Dependency);
                        module.AddDependency(keymodule == null ? new ModuleInformation(dependency.Dependency, "", null) : modules[keymodule]);
                        break;
                    case DependencyType.Type:
                        IModule typemodule = modules.Values.Where(m => m.Type == dependency.Dependency).Select(m => m.Module).FirstOrDefault();
                        module.AddDependency(typemodule == null ? new ModuleInformation("", dependency.Dependency, null) : modules[typemodule]);
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
                        if (module.IsRunning)
                            continue;

                        Logger.Info(this, $"Activating '{module.Type}'");
                        try
                        {
                            StartModule(module);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(this, "Unable to activate module", e);
                        }
                    }
                    else
                    {
                        if (!module.IsRunning)
                            continue;

                        Logger.Info(this, $"Stopping '{module.Type}'");
                        try
                        {
                            StopModule(module);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(this, "Unable to stop module", e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// starts a module
        /// </summary>
        /// <param name="module">module to start</param>
        protected virtual void StartModule(ModuleInformation module)
        {
            (module.Module as IRunnableModule)?.Start();
            module.SetRunning(true);
        }

        /// <summary>
        /// stops the module
        /// </summary>
        /// <param name="module">module to stop</param>
        protected virtual void StopModule(ModuleInformation module)
        {
            (module.Module as IRunnableModule)?.Stop();
            module.SetRunning(false);
        }

        /// <summary>
        /// start all managed modules
        /// </summary>
        public void Start() {
            InitializeModules();
            RecheckModuleState();
        }

        /// <summary>
        /// stop all managed modules
        /// </summary>
        public void Stop() {
            foreach (TMetaInformation module in modules.Values)
            {
                Logger.Info(this, $"Stopping '{module.Type}'");
                try
                {
                    if (module.IsRunning)
                    {
                        StopModule(module);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(this, $"Unable to stop '{module.Type}'", e);
                }
            }
        }
    }
}