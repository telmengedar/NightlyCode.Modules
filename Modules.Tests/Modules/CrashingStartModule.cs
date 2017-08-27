using NightlyCode.Modules;

namespace Modules.Tests.Modules {
    public class CrashingStartModule : IRunnableModule {
        public void Start() {
            throw new System.NotImplementedException();
        }

        public void Stop() {
        }
    }
}