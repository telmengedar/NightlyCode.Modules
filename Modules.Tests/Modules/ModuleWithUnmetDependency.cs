using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [Dependency("missingmodule", DependencyType.Type)]
    public class ModuleWithUnmetDependency : IModule {
         
    }
}