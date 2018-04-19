namespace NightlyCode.Modules.Dependencies {

    /// <summary>
    /// type of module dependency
    /// </summary>
    public enum DependencyType {

        /// <summary>
        /// module has to be initialized after the specified module
        /// </summary>
        InitializeAfter,

        /// <summary>
        /// module has to be initialized before the specified module
        /// </summary>
        InitializeBefore
    }
}