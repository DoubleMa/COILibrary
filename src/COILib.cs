using COILib.General;
using COILib.Logging;
using Mafi;
using Mafi.Collections;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;
using System;

namespace COILib;

public sealed class COILib : IMod {
    public static readonly Version ModVersion = new(major: 1, minor: 0, build: 0);
    private static bool s_initialized;
    public string Name => "COILib";
    public int Version => 1;
    public bool IsUiOnly => false;

    public Option<IConfig> ModConfig { get; }

    public COILib() {
        initializeThis();
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

    private void initializeThis() {
        if (s_initialized) {
            return;
        }

        AnEarlyAssemblyLoader.Initialize();
        ExtLog.Info($"Current {Name} version v{ModVersion.ToString(3)}", true);
        s_initialized = true;
    }
}