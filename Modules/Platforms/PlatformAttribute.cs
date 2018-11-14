using System;

namespace NightlyCode.Modules.Platforms {

    /// <summary>
    /// attribute for <see cref="IModule"/> to specify the module to run on
    /// </summary>
    /// <remarks>
    /// it is assumed that modules run on <see cref="ModulePlatform.Any"/> platform
    /// modules forced to run on <see cref="ModulePlatform.X64"/> are not run if os is x86
    /// thus modules depending on these modules can't be started as well
    /// </remarks>
    public class PlatformAttribute : Attribute {

        /// <summary>
        /// creates a new <see cref="PlatformAttribute"/>
        /// </summary>
        /// <param name="platform">platform to run module on</param>
        public PlatformAttribute(ModulePlatform platform) {
            Platform = platform;
        }

        /// <summary>
        /// platform to run module on
        /// </summary>
        public ModulePlatform Platform { get; }
    }
}