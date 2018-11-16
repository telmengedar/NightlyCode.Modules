using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    [Module(AutoCreate = true)]
    public class CircularModule1 {
        CircularModule3 c3;

        public CircularModule1(CircularModule3 c3) {
            this.c3 = c3;
        }
    }
}