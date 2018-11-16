using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    public class DependendModule1 {
        DependendModule3 m3;

        public DependendModule1(DependendModule3 m3) {
            this.m3 = m3;
        }
    }
}