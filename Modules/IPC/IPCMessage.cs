namespace NightlyCode.Modules.IPC {

    /// <summary>
    /// message for ipc communication
    /// </summary>
    public class IPCMessage {

        /// <summary>
        /// id of message
        /// </summary>
        public ushort ID { get; set; }

        /// <summary>
        /// message to send
        /// </summary>
        public string Message { get; set; } 
    }
}