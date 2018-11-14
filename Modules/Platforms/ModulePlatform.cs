namespace NightlyCode.Modules.Platforms {

    /// <summary>
    /// platform a module is running on
    /// </summary>
    public enum ModulePlatform {

        /// <summary>
        /// module runs on any platform
        /// </summary>
        Any,

        /// <summary>
        /// module has to run on 32 bit
        /// </summary>
        X86,

        /// <summary>
        /// module has to run on 64 bit
        /// </summary>
        X64
    }
}