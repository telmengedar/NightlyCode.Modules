using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modules.Tests.Modules;
using NightlyCode.Modules;

namespace Modules.Tests
{
    [TestClass]
    public class ModuleManagerTests
    {

        [TestMethod]
        public void ModuleWithUnmetDependencyIsntInitialized() {
            ModuleManager<ModuleInformation> modulemanager=new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(new ModuleWithUnmetDependency());
            modulemanager.Start();

            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<ModuleWithUnmetDependency>()).IsInitialized);
            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<ModuleWithUnmetDependency>()).IsRunning);
        }

        [TestMethod]
        public void CircularReferencedModulesAreNotInitialized() {
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(new CircularModule1());
            modulemanager.AddModule(new CircularModule2());
            modulemanager.AddModule(new CircularModule3());
            modulemanager.Start();

            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<CircularModule1>()).IsInitialized);
            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<CircularModule1>()).IsRunning);
            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<CircularModule2>()).IsInitialized);
            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<CircularModule2>()).IsRunning);
            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<CircularModule3>()).IsInitialized);
            Assert.AreEqual(false, modulemanager.GetInformation(modulemanager.GetModule<CircularModule3>()).IsRunning);
        }

        [TestMethod]
        public void StartASimpleModule() {
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(new SimpleModule());
            modulemanager.Start();

            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<SimpleModule>()).IsInitialized);
            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<SimpleModule>()).IsRunning);
        }

        [TestMethod]
        public void StartDependendModulesInCorrectOrder() {
            Type[] expectedorder = {
                typeof(DependendModule3),
                typeof(DependendModule1),
                typeof(DependendModule2),
            };

            List<Type> actualorder=new List<Type>();

            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
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
            ModuleManager<ModuleInformation> modulemanager = new ModuleManager<ModuleInformation>();
            modulemanager.AddModule(new DependendModule4());
            modulemanager.AddModule(new DependendModule1());
            modulemanager.AddModule(new DependendModule3());
            modulemanager.Start();

            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<DependendModule1>()).IsInitialized);
            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<DependendModule1>()).IsRunning);
            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<DependendModule3>()).IsInitialized);
            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<DependendModule3>()).IsRunning);
            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<DependendModule4>()).IsInitialized);
            Assert.AreEqual(true, modulemanager.GetInformation(modulemanager.GetModule<DependendModule4>()).IsRunning);
        }
    }
}
