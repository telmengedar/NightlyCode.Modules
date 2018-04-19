using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency(nameof(DependendModule3), SpecifierType.Type)]
    public class DependendModule1 : IModule {
         
    }
}