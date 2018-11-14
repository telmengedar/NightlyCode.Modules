namespace NightlyCode.Modules {

    /// <summary>
    /// interface for an <see cref="IModule"/> able to process commands
    /// </summary>
    public interface ICommandModule : IModule {

        /// <summary>
        /// processes a command
        /// </summary>
        /// <param name="command">command to execute</param>
        /// <param name="arguments">command arguments (first is command itself)</param>
        void ProcessCommand(string command, params string[] arguments);
    }
}