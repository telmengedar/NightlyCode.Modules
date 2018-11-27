using Modules.Tests.Modules;
using NightlyCode.Core.Script;
using NightlyCode.Modules;
using NightlyCode.Modules.Scripts;
using NUnit.Framework;

namespace Modules.Tests {

    [TestFixture]
    public class ModuleCommandTests {

        [Test]
        public void SetStringProperty() {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));

            parser.Parse("testcall.teststring=rudellopf").Execute();
            Assert.AreEqual("rudellopf", modulemanager.GetModule<TestCallModule>().TestString);
        }

        [Test]
        public void SetIntProperty()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));
            parser.Parse("testcall.testint=822").Execute();
            Assert.AreEqual(822, modulemanager.GetModule<TestCallModule>().TestInt);
        }

        [Test]
        public void SetDoubleProperty()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));
            parser.Parse("testcall.testdouble=728.3").Execute();
            Assert.AreEqual(728.3, modulemanager.GetModule<TestCallModule>().TestDouble);
        }

        [Test]
        public void SetEnumProperty()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));
            parser.Parse("testcall.testenum=primary").Execute();
            Assert.AreEqual(TestEnum.Primary, modulemanager.GetModule<TestCallModule>().TestEnum);
        }

        [Test]
        public void TestMethod()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));
            parser.Parse("testcall.testmethod()").Execute();
            Assert.AreEqual("TestMethod", modulemanager.GetModule<TestCallModule>().LastMethodCalled);
        }

        [Test]
        public void TestMethodWithAnArgument()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));
            parser.Parse("testcall.testmethod2(something)").Execute();
            Assert.AreEqual("TestMethod2", modulemanager.GetModule<TestCallModule>().LastMethodCalled);
        }

        [Test]
        public void TestMethodWith3Arguments()
        {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<TestCallModule>();
            modulemanager.Start();

            ScriptParser parser = new ScriptParser(new ContextScriptHost(modulemanager));
            parser.Parse("testcall.testmethod3(something,144,232.1)").Execute();
            Assert.AreEqual("TestMethod3", modulemanager.GetModule<TestCallModule>().LastMethodCalled);
        }
    }
}