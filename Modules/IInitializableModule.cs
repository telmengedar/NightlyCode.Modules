namespace NightlyCode.Modules {

    /// <summary>
    /// interface for a <see cref="IModule"/> which is to be initialized before start
    /// </summary>
    /// <remarks>
    /// initialization is done only once during the lifetime of a module (start/stop is done every time the module is started or stopped)
    /// </remarks>
    public interface IInitializableModule : IModule
    {

        /// <summary>
        /// initializes the <see cref="IModule"/> to prepare for start
        /// </summary>
        void Initialize();
    }
}