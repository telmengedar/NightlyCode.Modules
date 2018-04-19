namespace NightlyCode.Modules.Dependencies {

    /// <summary>
    /// type of dependency specification
    /// </summary>
    public enum SpecifierType {

        /// <summary>
        /// type name
        /// </summary>
        Type,

        /// <summary>
        /// key of <see cref="IModule"/> if any
        /// </summary>
        Key
    }
}