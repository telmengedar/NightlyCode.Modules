using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NightlyCode.Modules.Context;
using NightlyCode.Modules.Logging;

namespace NightlyCode.Modules {

    /// <summary>
    /// scans for module types in assemblies
    /// </summary>
    public class ModuleScanner {
        readonly string path;

        /// <summary>
        /// creates a new <see cref="ModuleScanner"/>
        /// </summary>
        /// <param name="path">path in which to scan for modules</param>
        public ModuleScanner(string path) {
            this.path = path;
        }

        /// <summary>
        /// scans for modules in a path
        /// </summary>
        /// <param name="context">context used to initialize module</param>
        /// <param name="predicate">predicate used to check whether to include module in scan result (optional)</param>
        /// <returns></returns>
        public IEnumerable<IModule> ScanForModules(IModuleContext context, Func<Type, bool> predicate=null) {
            Logger.Info(this, $"Scanning '{path}' for modules");

            if(!Directory.Exists(path)) {
                Logger.Warning(this, $"Path '{path}' does not exist.");
                yield break;
            }

            if(predicate == null)
                predicate = type => true;

            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            foreach (string assemblyfile in Directory.GetFiles(path, "*.dll")) {
                Type[] types;
                try {
                    Assembly assembly = Assembly.LoadFile(assemblyfile);
                    types = assembly.GetTypes();
                }
                catch(Exception e) {
                    Logger.Warning(this, $"Unable to load '{assemblyfile}'", e.Message);
                    continue;
                }

                foreach(Type type in types) {
                    if(type.IsInterface || type.GetInterfaces().All(i => i != typeof(IModule)) || !predicate(type))
                        continue;

                    IModule module;
                    try {
                        module = (IModule)Activator.CreateInstance(type, context);
                    }
                    catch(Exception e) {
                        Logger.Warning(this, $"Unable to create '{type.FullName}'", e.Message);
                        continue;
                    }

                    yield return module;
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
            string assemblypath = Path.Combine(path, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblypath))
                return null;
            return Assembly.LoadFrom(assemblypath);
        }
    }
}