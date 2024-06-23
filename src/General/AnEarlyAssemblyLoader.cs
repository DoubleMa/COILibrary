using COILib.Logging;
using System;
using System.IO;
using System.Reflection;

namespace COILib.General;

public static class AnEarlyAssemblyLoader {

	static AnEarlyAssemblyLoader() {
		loadAssemblies(true);
	}

	internal static void Initialize() {
		//AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
		//System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(GithubManager).TypeHandle);
	}

	private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args) {
		string assemblyName = new AssemblyName(args.Name).Name + ".dll";
		string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dependencies", assemblyName);
		return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
	}

	public static bool LoadAssemblies() => loadAssemblies(false);

	private static bool loadAssemblies(bool current) {
		return Static.TryRun(() => {
			bool result = true;
			string pluginsPath = Static.GetCallingAssemblyLocation(current, "Managed");

			if (!Directory.Exists(pluginsPath)) {
				return false;
			}

			foreach (string file in Directory.GetFiles(pluginsPath)) {
				if (file.EndsWith(".dll")) {
					result = result && loadAssembly(Path.Combine(pluginsPath, file));
				}
			}

			return result;
		});
	}

	private static bool loadAssembly(string path) {
		try {
			if (File.Exists(path)) {
				ExtLog.Info($"Trying to Loading Assembly: {path}", true);
				Assembly.LoadFrom(path);
				return true;
			}

			ExtLog.Error($"Couldn't find {path}", true);
		}
		catch (Exception ex) {
			ExtLog.Error($"Failed to load {path}: {ex.Message}", true);
		}
		return false;
	}
}