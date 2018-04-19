using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Dependency("missingmodule", SpecifierType.Type)]
    public class ModuleWithUnmetDependency : IModule {
         
    }
}