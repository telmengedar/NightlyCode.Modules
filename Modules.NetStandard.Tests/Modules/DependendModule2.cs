using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    public class DependendModule2 {
        DependendModule1 m1;

        public DependendModule2(DependendModule1 m1) {
            this.m1 = m1;
        }
    }
}