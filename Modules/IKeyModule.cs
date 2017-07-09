namespace NightlyCode.Modules {

    /// <summary>
    /// interface for a <see cref="IModule"/> with a key for retrieval
    /// </summary>
    /// <remarks>
    /// key is usually used to refer to this module instead of type name
    /// can also be used to use different module implementations based on a key
    /// </remarks>
    public interface IKeyModule : IModule {

        /// <summary>
        /// key of module
        /// </summary>
        string Key { get; }
    }
}