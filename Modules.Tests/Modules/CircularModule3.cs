using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {
    [Dependency(nameof(CircularModule2), SpecifierType.Type)]
    public class CircularModule3 : IModule {
         
    }
}