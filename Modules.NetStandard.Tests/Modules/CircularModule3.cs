using NightlyCode.Modules;
using NightlyCode.Modules.Dependencies;

namespace Modules.Tests.Modules {
    public class CircularModule3 {
        CircularModule2 c2;

        public CircularModule3(CircularModule2 c2) {
            this.c2 = c2;
        }
    }
}