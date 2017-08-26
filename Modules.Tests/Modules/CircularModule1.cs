using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency(nameof(CircularModule3), DependencyType.Type)]
    public class CircularModule1 : IModule {
         
    }
}