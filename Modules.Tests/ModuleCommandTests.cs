using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modules.Tests.Modules;
using NightlyCode.Modules;

namespace Modules.Tests {

    [TestClass]
    public class ModuleCommandTests {

        [TestMethod]
        public void SetStringProperty() {
            TestCallModule module=new TestCallModule();
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(module);
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.teststring=rudellopf");
            Assert.AreEqual("rudellopf", module.TestString);
        }

        [TestMethod]
        public void SetIntProperty()
        {
            TestCallModule module = new TestCallModule();
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(module);
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testint=822");
            Assert.AreEqual(822, module.TestInt);
        }

        [TestMethod]
        public void SetDoubleProperty()
        {
            TestCallModule module = new TestCallModule();
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(module);
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testdouble=728.3");
            Assert.AreEqual(728.3, module.TestDouble);
        }

        [TestMethod]
        public void TestMethod()
        {
            TestCallModule module = new TestCallModule();
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(module);
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testmethod()");
            Assert.AreEqual("TestMethod", module.LastMethodCalled);
        }

        [TestMethod]
        public void TestMethodWithAnArgument()
        {
            TestCallModule module = new TestCallModule();
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(module);
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testmethod2(something)");
            Assert.AreEqual("TestMethod2", module.LastMethodCalled);
        }

        [TestMethod]
        public void TestMethodWith3Arguments()
        {
            TestCallModule module = new TestCallModule();
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(module);
            modulemanager.Start();

            modulemanager.ExecuteCommand("testcall.testmethod3(something,144,232.1)");
            Assert.AreEqual("TestMethod3", module.LastMethodCalled);
        }
    }
}