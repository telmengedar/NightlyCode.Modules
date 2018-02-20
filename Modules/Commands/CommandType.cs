namespace NightlyCode.Modules.Commands {

    /// <summary>
    /// type of parsed command
    /// </summary>
    public enum CommandType {

        /// <summary>
        /// no recognized command
        /// </summary>
        None=0,

        /// <summary>
        /// a property value shall be set
        /// </summary>
        Property=1,

        /// <summary>
        /// a method is to be called
        /// </summary>
        Method=2
    }
}