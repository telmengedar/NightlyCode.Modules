using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency(nameof(DependendModule1), SpecifierType.Type)]
    public class DependendModule2 : IModule {
         
    }
}