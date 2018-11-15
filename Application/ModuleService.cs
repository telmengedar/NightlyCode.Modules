using System;
using System.IO;
using NightlyCode.Core.ComponentModel;
using NightlyCode.Core.Configuration.Ini;
using NightlyCode.Core.Helpers;
using NightlyCode.Core.Logs;
using NightlyCode.Core.Services;
using NightlyCode.Modules.Context;

namespace NightlyCode.Modules.Application {

    /// <summary>
    /// service creating a module host
    /// </summary>
    public class ModuleService : WindowsService {
        readonly LogFileCleaner logcleaner = new LogFileCleaner(Path.Combine(PathExtensions.GetApplicationDirectory(), "logs"));
        IModuleContext context;

        static ModuleService() {
            Logger.Message += new FileLogger(Path.Combine(PathExtensions.GetApplicationDirectory(), $"logs/{Path.ChangeExtension(Path.GetFileName(PathExtensions.GetApplicationExecutable()), "log")}")).Log;
        }

        protected override void OnStart(string[] args) {
            base.OnStart(args);
            logcleaner.Start(TimeSpan.FromMinutes(30.0));

            string contexttypename = null;

            string inifile = Path.ChangeExtension(PathExtensions.GetApplicationExecutable(), "ini");
            if(File.Exists(inifile)) {
                IniFile ini = IniFile.Load(inifile);
                contexttypename = ini["service"]["context"];
            }
            else Logger.Info(this, $"No configuration file '{inifile}' found, using default settings.");

            if(!string.IsNullOrEmpty(contexttypename)) {
                Type contexttype = Type.GetType(contexttypename);
                if(contexttype != null) {
                    context = (IModuleContext)Activator.CreateInstance(contexttype);
                }
                else {
                    Logger.Error(this, $"Context type '{contexttypename}' not found.");
                    throw new Exception("Missing module context");
                }
            }
            else Logger.Info(this, "No context type specified in configuration 'service.context', using default type.");

            if (context == null)
                context = new ModuleContext<ModuleInformation>();

            Arguments arguments = Arguments.Parse(args);

            ModuleScanner scanner = new ModuleScanner(Path.Combine(PathExtensions.GetApplicationDirectory(), "modules"));
            foreach (IModule module in scanner.ScanForModules(context))
                context.AddModule(module);

            context.Start();
        }

        protected override void OnStop() {
            base.OnStop();
            context.Stop();
            logcleaner.Stop();
        }
    }
}