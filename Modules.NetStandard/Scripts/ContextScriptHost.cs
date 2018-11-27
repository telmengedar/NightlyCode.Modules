using System.Linq;
using NightlyCode.Core.Script;

namespace NightlyCode.Modules.Scripts {

    /// <summary>
    /// serves modules as hosts to script commands
    /// </summary>
    public class ContextScriptHost : IScriptHostPool {
        readonly IModuleContext context;

        /// <summary>
        /// creates a new <see cref="ContextScriptHost"/>
        /// </summary>
        /// <param name="context">context from which to get modules</param>
        public ContextScriptHost(IModuleContext context) {
            this.context = context;
        }

        /// <inheritdoc />
        public object GetHost(string name) {
            name = name.ToLower();
            if(context.TryGetModuleByKey(name, out object module))
                return module;
            ModuleInformation moduleinfo = context.Modules.FirstOrDefault(m => m.TypeName.ToLower() == name);
            if(moduleinfo == null)
                throw new ModuleNotFoundException($"There is no module with key or typename '{name}'");
            return context.GetModule(moduleinfo.Type);
        }
    }
}