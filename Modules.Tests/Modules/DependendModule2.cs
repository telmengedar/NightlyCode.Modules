using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency(nameof(DependendModule1), DependencyType.Type)]
    public class DependendModule2 : IModule {
         
    }
}