using NightlyCode.Modules.IPC;

namespace NightlyCode.Modules.Context {

    /// <summary>
    /// interceptor accessing modules of another process
    /// </summary>
    public class ModuleInterceptor : IInterceptor {
        readonly IPCModule ipc;
        readonly string targetnode;
        readonly string module;
        

        /// <summary>
        /// interceptor for module calls
        /// </summary>
        /// <param name="ipc">module used to access modules on other nodes</param>
        /// <param name="targetnode">node to access</param>
        /// <param name="module">module to call</param>
        public ModuleInterceptor(IPCModule ipc, string targetnode, string module) {
            this.ipc = ipc;
            this.targetnode = targetnode;
            this.module = module;
            
        }

        public void SetProperty(string property, object value) {
            ipc.SendMessage(targetnode, $"{module}.{property}={value}");
        }

        public object GetProperty(string property) {
            throw new System.NotImplementedException();
        }

        public object CallMethod(string method, object[] parameters) {
            ipc.SendMessage(targetnode, $"{module}.{method}({string.Join(",", parameters)})");
            return null;
        }
    }
}