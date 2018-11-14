using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modules.Tests.Modules;
using NightlyCode.Modules;
using NightlyCode.Modules.Context;

namespace Modules.Tests
{
    [TestClass]
    public class ModuleManagerTests
    {

        [TestMethod]
        public void ModuleWithUnmetDependencyIsntInitialized() {
            ModuleContext<ModuleInformation> modulemanager=new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new ModuleWithUnmetDependency());
            modulemanager.Start();

            Assert.AreEqual(ModuleStatus.ErrorInitializing, modulemanager.GetInformation(modulemanager.GetModule<ModuleWithUnmetDependency>()).Status);
        }

        [TestMethod]
        public void CircularReferencedModulesAreNotInitialized() {
            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new CircularModule1());
            modulemanager.AddModule(new CircularModule2());
            modulemanager.AddModule(new CircularModule3());
            modulemanager.Start();

            Assert.AreEqual(ModuleStatus.ErrorInitializing, modulemanager.GetInformation(modulemanager.GetModule<CircularModule1>()).Status);
            Assert.AreEqual(ModuleStatus.ErrorInitializing, modulemanager.GetInformation(modulemanager.GetModule<CircularModule2>()).Status);
            Assert.AreEqual(ModuleStatus.ErrorInitializing, modulemanager.GetInformation(modulemanager.GetModule<CircularModule3>()).Status);
        }

        [TestMethod]
        public void StartASimpleModule() {
            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new SimpleModule());
            modulemanager.Start();

            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<SimpleModule>()).Status);
        }

        [TestMethod]
        public void StartDependendModulesInCorrectOrder() {
            Type[] expectedorder = {
                typeof(DependendModule3),
                typeof(DependendModule1),
                typeof(DependendModule2),
            };

            List<Type> actualorder=new List<Type>();

            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new DependendModule1());
            modulemanager.AddModule(new DependendModule2());
            modulemanager.AddModule(new DependendModule3());
            modulemanager.ModuleStarted += module => actualorder.Add(module.GetType());
            modulemanager.Start();

            Assert.AreEqual(expectedorder.Length, actualorder.Count);
            for(int i = 0; i < expectedorder.Length; ++i)
                Assert.AreEqual(expectedorder[i], actualorder[i], "Module activation order did not match expected order");
        }

        [TestMethod]
        public void TwoModuleDependOnOneOther() {
            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new DependendModule4());
            modulemanager.AddModule(new DependendModule1());
            modulemanager.AddModule(new DependendModule3());
            modulemanager.Start();

            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<DependendModule1>()).Status);
            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<DependendModule3>()).Status);
            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<DependendModule4>()).Status);
        }

        [TestMethod]
        public void CrashingStartModule() {
            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new CrashingStartModule());
            modulemanager.Start();

            Assert.AreEqual(ModuleStatus.ErrorStarting, modulemanager.GetInformation(modulemanager.GetModule<CrashingStartModule>()).Status);
        }

        [TestMethod]
        public void CrashingStartCancelsAllDependendStarts()
        {
            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new CrashingStartModule());
            modulemanager.AddModule(new DependendOnCrashModule());
            modulemanager.Start();

            Assert.AreEqual(ModuleStatus.ErrorStarting, modulemanager.GetInformation(modulemanager.GetModule<CrashingStartModule>()).Status);
            Assert.AreEqual(ModuleStatus.Initialized, modulemanager.GetInformation(modulemanager.GetModule<DependendOnCrashModule>()).Status);
        }

        [TestMethod]
        public void StoppingModuleStopsDependendModules() {
            ModuleContext<ModuleInformation> modulemanager = new ModuleContext<ModuleInformation>();
            modulemanager.AddModule(new DependendModule1());
            modulemanager.AddModule(new DependendModule2());
            modulemanager.AddModule(new DependendModule3());
            modulemanager.Start();


            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<DependendModule1>()).Status);
            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<DependendModule2>()).Status);
            Assert.AreEqual(ModuleStatus.Started, modulemanager.GetInformation(modulemanager.GetModule<DependendModule3>()).Status);

            modulemanager.StopModule<DependendModule3>();

            Assert.AreEqual(ModuleStatus.Stopped, modulemanager.GetInformation(modulemanager.GetModule<DependendModule1>()).Status);
            Assert.AreEqual(ModuleStatus.Stopped, modulemanager.GetInformation(modulemanager.GetModule<DependendModule2>()).Status);
            Assert.AreEqual(ModuleStatus.Stopped, modulemanager.GetInformation(modulemanager.GetModule<DependendModule3>()).Status);
        }
    }
}
