using System;
using Modules.Tests.Modules;
using NightlyCode.Modules;
using NUnit.Framework;

namespace Modules.Tests
{
    [TestFixture]
    public class ModuleManagerTests
    {

        [Test]
        public void CircularReferencedModulesAreNotInitialized() {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<CircularModule1>();
            modulemanager.AddModule<CircularModule2>();
            modulemanager.AddModule<CircularModule3>();
            modulemanager.Start();

            Assert.IsFalse(modulemanager.IsStarted<CircularModule1>());
            Assert.IsFalse(modulemanager.IsStarted<CircularModule2>());
            Assert.IsFalse(modulemanager.IsStarted<CircularModule3>());
        }

        [Test]
        public void StartASimpleModule() {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<SimpleModule>();
            modulemanager.Start();

            Assert.NotNull(modulemanager.GetModule<SimpleModule>());
        }

        [Test]
        public void TwoModuleDependOnOneOther() {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<DependendModule4>();
            modulemanager.AddModule<DependendModule1>();
            modulemanager.AddModule<DependendModule3>();
            modulemanager.Start();

            Assert.NotNull(modulemanager.GetModule<DependendModule4>());
            Assert.NotNull(modulemanager.GetModule<DependendModule1>());
            Assert.NotNull(modulemanager.GetModule<DependendModule3>());
        }

        [Test]
        public void CrashingStartModule() {
            ModuleContext modulemanager = new ModuleContext();
            modulemanager.AddModule<CrashingStartModule>();
            modulemanager.Start();

            Assert.Throws<NotImplementedException>(() => modulemanager.GetModule<CrashingStartModule>());
        }

        [Test]
        public void ProvideRealContext()
        {
            ModuleContext modulemanager = new ModuleContext();
            IModuleContext context = modulemanager.GetModule<IModuleContext>();
            Assert.AreEqual(modulemanager, context);
        }

    }
}
