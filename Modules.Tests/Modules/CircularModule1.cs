using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency(nameof(CircularModule3), SpecifierType.Type)]
    public class CircularModule1 : IModule {
         
    }
}