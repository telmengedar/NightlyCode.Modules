using Modules.Tests.Modules;
using NightlyCode.Modules;
using NUnit.Framework;

namespace Modules.Tests {

    [TestFixture]
    public class ModuleCommandTests {

        [Test]
        public void SetStringProperty() {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.teststring=rudellopf");
            Assert.AreEqual("rudellopf", modulemanager.GetModule<TestCallModule>().TestString);
        }

        [Test]
        public void SetIntProperty()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testint=822");
            Assert.AreEqual(822, modulemanager.GetModule<TestCallModule>().TestInt);
        }

        [Test]
        public void SetDoubleProperty()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testdouble=728.3");
            Assert.AreEqual(728.3, modulemanager.GetModule<TestCallModule>().TestDouble);
        }

        [Test]
        public void SetEnumProperty()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testenum=primary");
            Assert.AreEqual(TestEnum.Primary, modulemanager.GetModule<TestCallModule>().TestEnum);
        }

        [Test]
        public void TestMethod()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testmethod()");
            Assert.AreEqual("TestMethod", modulemanager.GetModule<TestCallModule>().LastMethodCalled);
        }

        [Test]
        public void TestMethodWithAnArgument()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testmethod2(something)");
            Assert.AreEqual("TestMethod2", modulemanager.GetModule<TestCallModule>().LastMethodCalled);
        }

        [Test]
        public void TestMethodWith3Arguments()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testmethod3(something,144,232.1)");
            Assert.AreEqual("TestMethod3", modulemanager.GetModule<TestCallModule>().LastMethodCalled);
        }
    }
}