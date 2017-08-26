using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency(nameof(CircularModule1), DependencyType.Type)]
    public class CircularModule2 : IModule {
         
    }
}