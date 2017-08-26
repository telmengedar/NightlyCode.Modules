using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency(nameof(DependendModule1), DependencyType.Type)]
    [Dependency(nameof(DependendModule3), DependencyType.Type)]
    public class DependendModule4 : IModule {
         
    }
}