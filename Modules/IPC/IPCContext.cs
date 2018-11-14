using System.IO.Pipes;

namespace NightlyCode.Modules.IPC {

    /// <summary>
    /// context for ipc communication
    /// </summary>
    public class IPCContext {

        /// <summary>
        /// name of target node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// buffer for sent bytes
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// client used to send data
        /// </summary>
        public NamedPipeClientStream Client { get; set; }

        /// <summary>
        /// lock used for synchronization
        /// </summary>
        public object Lock { get; private set; } = new object();
    }
}