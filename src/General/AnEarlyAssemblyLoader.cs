using COILib.Logging;
using System;
using System.IO;
using System.Reflection;

namespace COILib.General {

    public static class AnEarlyAssemblyLoader {

        static AnEarlyAssemblyLoader() {
            LoadAssemblies(true);
        }

        internal static void Initialize() {
            //AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            //System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(GithubManager).TypeHandle);
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args) {
            string assemblyName = new AssemblyName(args.Name).Name + ".dll";
            string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dependencies", assemblyName);
            if (File.Exists(assemblyPath)) return Assembly.LoadFrom(assemblyPath);
            return null;
        }

        public static bool LoadAssemblies() => LoadAssemblies(false);

        private static bool LoadAssemblies(bool current) {
            return Static.TryRun(() => {
                var result = true;
                var pluginsPath = Static.GetCallingAssemblyLocation(current, "Managed");
                if (Directory.Exists(pluginsPath))
                    foreach (var file in Directory.GetFiles(pluginsPath))
                        if (file.EndsWith(".dll"))
                            result = result && LoadAssembly(Path.Combine(pluginsPath, file));
                return result;
            });
        }

        private static bool LoadAssembly(string path) {
            try {
                if (File.Exists(path)) {
                    ExtLog.Info($"Trying to Loading Assembly: {path}", true);
                    Assembly.LoadFrom(path);
                    return true;
                }
                else ExtLog.Error($"Couldn't find {path}", true);
            }
            catch (Exception ex) {
                ExtLog.Error($"Failed to load {path}: {ex.Message}", true);
            }
            return false;
        }
    }
}