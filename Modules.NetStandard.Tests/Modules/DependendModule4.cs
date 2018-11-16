
namespace Modules.Tests.Modules {

    public class DependendModule4 {
        DependendModule1 m1;
        DependendModule3 m3;

        public DependendModule4(DependendModule1 m1, DependendModule3 m3) {
            this.m1 = m1;
            this.m3 = m3;
        }
    }
}