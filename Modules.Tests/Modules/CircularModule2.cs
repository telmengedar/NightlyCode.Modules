using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency(nameof(CircularModule1), SpecifierType.Type)]
    public class CircularModule2 : IModule {
         
    }
}