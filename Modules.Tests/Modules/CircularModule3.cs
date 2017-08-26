using NightlyCode.Modules;

namespace Modules.Tests.Modules {
    [Dependency(nameof(CircularModule2), DependencyType.Type)]
    public class CircularModule3 : IModule {
         
    }
}