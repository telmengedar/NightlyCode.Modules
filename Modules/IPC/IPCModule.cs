using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using NightlyCode.Core.Logs;

namespace NightlyCode.Modules.IPC {

    /// <summary>
    /// host for ipc communication
    /// </summary>
    public class IPCModule : IRunnableModule {
        readonly string name;
        NamedPipeServerStream server;
        bool running = false;
        readonly byte[] buffer = new byte[4096];

        readonly object connectionlock = new object();
        readonly Dictionary<string, IPCContext> connections=new Dictionary<string, IPCContext>();

        /// <summary>
        /// creates a new <see cref="IPCModule"/>
        /// </summary>
        /// <param name="name"></param>
        public IPCModule(string name) {
            this.name = name;
        }

        /// <summary>
        /// triggered when a message was received over pipe
        /// </summary>
        public event Action<string> MessageReceived;

        IPCContext GetConnection(string name) {
            IPCContext context;
            lock(connectionlock) {
                if(!connections.TryGetValue(name, out context)) {
                    connections[name] = context = new IPCContext {
                        Name = name,
                        Client = new NamedPipeClientStream(".", $"{Process.GetCurrentProcess().ProcessName}_{name}", PipeDirection.Out)
                    };
                    context.Client.Connect(5000);
                }
            }
            return context;
        }

        /// <summary>
        /// starts the <see cref="IModule"/>
        /// </summary>
        public void Start() {
            server = new NamedPipeServerStream($"{Process.GetCurrentProcess().ProcessName}_{name}", PipeDirection.In, 5, PipeTransmissionMode.Byte);
            running = true;
            new Task(ReadMessages).Start();
        }

        /// <summary>
        /// stops the <see cref="IModule"/>
        /// </summary>
        public void Stop() {
            server.Dispose();
            running = false;
        }

        /// <summary>
        /// sends a message to a named ipc target
        /// </summary>
        /// <param name="target">name of target</param>
        /// <param name="message">message to send</param>
        public void SendMessage(string target, string message) {
            IPCContext context = GetConnection(target);
            lock(context.Lock) {
                byte[] messagedata = Encoding.UTF8.GetBytes(message);
                byte[] size = BitConverter.GetBytes(messagedata.Length);
                context.Client.Write(size, 0, 4);
                context.Client.Write(messagedata, 0, messagedata.Length);
            }
        }

        void ReadBlock(Stream stream, int count) {
            int offset = 0;
            int status;
            while(offset < count) {
                status = stream.Read(buffer, offset, count - offset);
                if(status <= 0)
                    throw new Exception("Pipe broken");
                offset += status;
            }
        }

        int ReadInt(Stream stream) {
            ReadBlock(stream, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        string ReadMessage(Stream stream) {
            int length = ReadInt(stream);
            ReadBlock(stream, length);
            return Encoding.UTF8.GetString(buffer, 0, length);
        }

        void ReadMessages() {
            try {
                server.WaitForConnection();
                while (running) {
                    MessageReceived?.Invoke(ReadMessage(server));
                }
            }
            catch (Exception e) {
                Logger.Error(this, "Unable to wait for ipc connection", e);
            }
        }
    }
}