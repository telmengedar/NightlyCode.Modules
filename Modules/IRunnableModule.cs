namespace NightlyCode.Modules {
    /// <summary>
    /// module with background processes to run
    /// </summary>
    public interface IRunnableModule : IModule {

        /// <summary>
        /// starts the <see cref="IModule"/>
        /// </summary>
        void Start();

        /// <summary>
        /// stops the <see cref="IModule"/>
        /// </summary>
        void Stop();
    }
}