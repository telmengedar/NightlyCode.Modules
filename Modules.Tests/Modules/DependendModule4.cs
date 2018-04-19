using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency(nameof(DependendModule1), SpecifierType.Type)]
    [Dependency(nameof(DependendModule3), SpecifierType.Type)]
    public class DependendModule4 : IModule {
         
    }
}