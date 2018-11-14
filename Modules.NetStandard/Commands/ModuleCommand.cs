namespace NightlyCode.Modules.Commands {

    /// <summary>
    /// command for module
    /// </summary>
    public class ModuleCommand {

        /// <summary>
        /// name of module
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// type of endpoint
        /// </summary>
        public CommandType Type { get; set; }

        /// <summary>
        /// name of endpoint
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// arguments for endpoint
        /// </summary>
        public string[] Arguments { get; set; }
    }
}