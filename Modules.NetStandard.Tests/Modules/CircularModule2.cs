using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {

    public class CircularModule2 {
        CircularModule1 c1;

        public CircularModule2(CircularModule1 c1) {
            this.c1 = c1;
        }
    }
}