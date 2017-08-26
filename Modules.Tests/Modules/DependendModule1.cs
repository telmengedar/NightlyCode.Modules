using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency(nameof(DependendModule3), DependencyType.Type)]
    public class DependendModule1 : IModule {
         
    }
}