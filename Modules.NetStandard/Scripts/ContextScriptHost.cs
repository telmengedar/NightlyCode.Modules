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
            return context.GetModuleByKey<object>(name);
        }
    }
}