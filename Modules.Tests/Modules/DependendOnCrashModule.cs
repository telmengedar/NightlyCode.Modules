using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency(nameof(CrashingStartModule), SpecifierType.Type)]
    public class DependendOnCrashModule : IModule {
    }
}