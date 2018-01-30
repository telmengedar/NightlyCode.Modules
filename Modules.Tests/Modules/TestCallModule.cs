using NightlyCode.Modules;

namespace Modules.Tests.Modules {

    [ModuleKey("testcall")]
    public class TestCallModule : IModule {

        public void TestMethod() {
            LastMethodCalled = nameof(TestMethod);
        }

        public void TestMethod2(string argument) {
            LastMethodCalled = nameof(TestMethod2);
        }

        public void TestMethod3(string argument1, int argument2, double argument3)
        {
            LastMethodCalled = nameof(TestMethod3);
        }

        public string LastMethodCalled { get; set; }

        public string TestString { get; set; }

        public int TestInt { get; set; }

        public double TestDouble { get; set; }
    }
}