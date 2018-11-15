namespace NightlyCode.Modules.Platforms {

    /// <summary>
    /// module which is configured to run on a specific platform
    /// </summary>
    public interface IPlatformModule {

        /// <summary>
        /// platform this module should run on
        /// </summary>
        ModulePlatform Platform { get; }
    }
}