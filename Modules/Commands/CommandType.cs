namespace NightlyCode.Modules.Commands {

    /// <summary>
    /// type of parsed command
    /// </summary>
    public enum CommandType {

        /// <summary>
        /// a property value shall be set
        /// </summary>
        Property,

        /// <summary>
        /// a method is to be called
        /// </summary>
        Method
    }
}