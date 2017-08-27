using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency(nameof(CrashingStartModule), DependencyType.Type)]
    public class DependendOnCrashModule : IModule {
    }
}