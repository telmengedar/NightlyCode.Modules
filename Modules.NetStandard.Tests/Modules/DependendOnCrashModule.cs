using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    public class DependendOnCrashModule {
        CrashingStartModule dependency;

        public DependendOnCrashModule(CrashingStartModule dependency) {
            this.dependency = dependency;
        }
    }
}