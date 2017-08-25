namespace NightlyCode.Modules {

    /// <summary>
    /// interface for an <see cref="IModule"/> able to process commands
    /// </summary>
    public interface ICommandModule : IModule {

        /// <summary>
        /// processes a command
        /// </summary>
        /// <param name="arguments">command arguments (first is command itself)</param>
        void ProcessCommand(string[] arguments);
    }
}