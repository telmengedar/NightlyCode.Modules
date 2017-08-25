using System;
using System.Linq;
using System.Reflection;

namespace NightlyCode.Modules.Logging {
    internal class Logger {
        static readonly Action<object, string, string> info = (s, m, d) => { };

        static readonly Action<object, string, string> warning = (s, m, d) => { };

        static readonly Action<object, string, Exception> error = (s, m, d) => { };

        static Logger() {
            if(Assembly.GetEntryAssembly()?.GetReferencedAssemblies()?.Any(a => a.Name == "NightlyCode.Core")??false) {
                ILoggerProvider nightlycodeprovider = (ILoggerProvider)Activator.CreateInstance(Type.GetType(typeof(Logger).Namespace + ".NightlyCodeLoggerInjector"));
                info = nightlycodeprovider.Info;
                warning = nightlycodeprovider.Warning;
                error = nightlycodeprovider.Error;
            }
        }

        public static void Info(object sender, string message, string details = null) {
            info(sender, message, details);
        }

        public static void Warning(object sender, string message, string details = null) {
            warning(sender, message, details);
        }

        public static void Error(object sender, string message, Exception details = null) {
            error(sender, message, details);
        }
    }

    internal interface ILoggerProvider {
        Action<object, string, string> Info { get; }

        Action<object, string, string> Warning { get; }

        Action<object, string, Exception> Error { get; }
    }

    internal class NightlyCodeLoggerInjector : ILoggerProvider {
        public Action<object, string, string> Info => Core.Logs.Logger.Info;
        public Action<object, string, string> Warning => Core.Logs.Logger.Warning;
        public Action<object, string, Exception> Error => Core.Logs.Logger.Error;
    }
}