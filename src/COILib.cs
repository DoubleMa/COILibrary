using COILib.General;
using COILib.Logging;
using Mafi;
using Mafi.Collections;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;
using System;

namespace COILib {

    public sealed class COILib : IMod {
        public static Version ModVersion = new Version(1, 0, 0);
        private static bool Initialized = false;
        public string Name => "COILib";
        public int Version => 1;
        public bool IsUiOnly => false;

        public Option<IConfig> ModConfig { get; }

        public COILib() {
            Init();
        }

        public void Initialize(DependencyResolver resolver, bool gameWasLoaded) {
        }

        public void ChangeConfigs(Lyst<IConfig> configs) {
        }

        public void RegisterPrototypes(ProtoRegistrator registrator) {
        }

        public void RegisterDependencies(DependencyResolverBuilder depBuilder, ProtosDb protosDb, bool wasLoaded) {
        }

        public void EarlyInit(DependencyResolver resolver) {
        }

        private void Init() {
            if (Initialized) return;
            AnEarlyAssemblyLoader.Initialize();
            ExtLog.Info($"Current {Name} version v{ModVersion.ToString(3)}", true);
            Initialized = true;
        }
    }
}